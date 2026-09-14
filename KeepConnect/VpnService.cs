using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace EasyConnectKeeper
{
    public class VpnService : IDisposable
    {
        private static readonly string[] PossibleProcessNames =
        {
            "EasyConnect",
            "ECAgent",
            "SangforCSClient",
            "CSClient",
            "SangforPromoteService",
            "svpn"
        };

        private CancellationTokenSource? _keepAliveCts;
        private Task? _keepAliveTask;

        public event Action<string>? OnLog;

        public string Host { get; set; } = "10.0.0.1";
        public int Port { get; set; } = 80;
        public int KeepAliveIntervalSeconds { get; set; } = 600; // 10 minutes default

        public bool IsKeepAliveActive => _keepAliveCts != null && !_keepAliveCts.IsCancellationRequested;

        /// <summary>
        /// 检测目标内网连通性，优先 TCP 端口检测，若端口不通则自动回退至 ICMP Ping 探测
        /// </summary>
        public async Task<(bool IsConnected, string Method)> CheckConnectivityWithDetailAsync()
        {
            string targetHost = Host.Trim();
            int targetPort = Port;

            if (string.IsNullOrWhiteSpace(targetHost))
                return (false, "主机未配置");

            // 1. 优先尝试 TCP 端口探测（2秒超时）
            try
            {
                using var client = new TcpClient();
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                await client.ConnectAsync(targetHost, targetPort, cts.Token);
                if (client.Connected)
                {
                    return (true, $"TCP:{targetPort} 连通");
                }
            }
            catch
            {
                // TCP 连接失败，继续尝试 Ping
            }

            // 2. 尝试 ICMP Ping 探测（2秒超时）
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(targetHost, 2000);
                if (reply.Status == IPStatus.Success)
                {
                    return (true, $"Ping 正常({reply.RoundtripTime}ms)");
                }
            }
            catch
            {
                // Ping 也失败
            }

            return (false, "目标不可达");
        }

        public async Task<bool> CheckConnectivityAsync()
        {
            var (isConnected, _) = await CheckConnectivityWithDetailAsync();
            return isConnected;
        }

        /// <summary>
        /// 开启后台保活引擎（异步心跳循环）
        /// </summary>
        public async Task StartKeepAliveAsync()
        {
            StopKeepAlive();

            _keepAliveCts = new CancellationTokenSource();
            var token = _keepAliveCts.Token;

            // 立即在后台启动心跳循环
            _keepAliveTask = Task.Run(() => KeepAliveLoopAsync(token), token);
            await Task.Yield();
        }

        public void StartKeepAlive()
        {
            _ = StartKeepAliveAsync();
        }

        private async Task KeepAliveLoopAsync(CancellationToken token)
        {
            OnLog?.Invoke($"已启动保活引擎，目标: {Host}:{Port}，心跳周期: {KeepAliveIntervalSeconds} 秒");

            int failedAttempts = 0;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var (success, detail) = await SendHeartbeatWithDetailAsync(token);
                    if (success)
                    {
                        failedAttempts = 0;
                        OnLog?.Invoke($"[心跳保持] 成功与 {Host}:{Port} 保持活跃 ({detail}) - {DateTime.Now:HH:mm:ss}");
                    }
                    else
                    {
                        failedAttempts++;
                        // 首次失败或每3次提醒一次，避免日志无意义刷屏
                        if (failedAttempts == 1 || failedAttempts % 3 == 0)
                        {
                            OnLog?.Invoke($"[心跳警告] 无法与 {Host}:{Port} 通信: {detail} (建议确认内网中是否存在该IP及对应端口)");
                        }

                        // 重试间隔 15 秒
                        await Task.Delay(15000, token);
                        continue;
                    }

                    int waitMs = Math.Max(KeepAliveIntervalSeconds * 1000, 5000);
                    await Task.Delay(waitMs, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnLog?.Invoke($"[心跳异常] {ex.Message}");
                    try
                    {
                        await Task.Delay(15000, token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        private async Task<(bool Success, string Detail)> SendHeartbeatWithDetailAsync(CancellationToken token)
        {
            string targetHost = Host.Trim();
            int targetPort = Port;
            string tcpError = "";
            string pingError = "";

            // 1. 尝试 TCP 连接并发送轻量探测数据包
            try
            {
                using var client = new TcpClient();
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(TimeSpan.FromSeconds(3));

                await client.ConnectAsync(targetHost, targetPort, cts.Token);
                if (client.Connected)
                {
                    using var stream = client.GetStream();
                    stream.WriteTimeout = 2000;
                    byte[] probeData = Encoding.ASCII.GetBytes($"HEAD / HTTP/1.1\r\nHost: {targetHost}\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(probeData, 0, probeData.Length, cts.Token);
                    return (true, $"TCP:{targetPort} 数据传输正常");
                }
            }
            catch (Exception ex)
            {
                tcpError = ex.Message;
            }

            // 2. 回退到 ICMP Ping
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(targetHost, 2000);
                if (reply.Status == IPStatus.Success)
                {
                    return (true, $"ICMP Ping 正常({reply.RoundtripTime}ms)");
                }
                else
                {
                    pingError = reply.Status.ToString();
                }
            }
            catch (Exception ex)
            {
                pingError = ex.Message;
            }

            return (false, $"TCP失败({tcpError}), Ping失败({pingError})");
        }

        public void StopKeepAlive()
        {
            if (_keepAliveCts != null)
            {
                try
                {
                    _keepAliveCts.Cancel();
                    _keepAliveCts.Dispose();
                }
                catch { }
                _keepAliveCts = null;
            }
        }

        /// <summary>
        /// 综合检测 EasyConnect 客户端进程是否存在（覆盖主程序及后台服务 ECAgent 等）
        /// </summary>
        public bool IsEasyConnectRunning()
        {
            foreach (var procName in PossibleProcessNames)
            {
                try
                {
                    if (Process.GetProcessesByName(procName).Length > 0)
                    {
                        return true;
                    }
                }
                catch { }
            }

            // 补充：检测主窗口标题
            try
            {
                foreach (var p in Process.GetProcesses())
                {
                    try
                    {
                        string title = p.MainWindowTitle;
                        if (!string.IsNullOrEmpty(title) &&
                            (title.Contains("EasyConnect", StringComparison.OrdinalIgnoreCase) ||
                             title.Contains("Easy Connect", StringComparison.OrdinalIgnoreCase)))
                        {
                            return true;
                        }
                    }
                    catch { }
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 获取 EasyConnect 启动路径，支持多源探测、注册表清理与默认目录兜底
        /// </summary>
        public string? GetEasyConnectInstallPath()
        {
            // 1. 优先从当前正在运行的进程中获取可执行文件路径
            foreach (var procName in PossibleProcessNames)
            {
                try
                {
                    var processes = Process.GetProcessesByName(procName);
                    foreach (var p in processes)
                    {
                        try
                        {
                            string? exePath = p.MainModule?.FileName;
                            if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                            {
                                string dir = Path.GetDirectoryName(exePath) ?? "";
                                string ecExe = Path.Combine(dir, "EasyConnect.exe");
                                if (File.Exists(ecExe)) return ecExe;
                                return exePath;
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // 2. 检查常见默认安装路径
            string[] defaultPaths =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Sangfor\SSL\EasyConnect\EasyConnect.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Sangfor\SSL\EasyConnect\EasyConnect.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Sangfor\EasyConnect\EasyConnect.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Sangfor\SSL\EasyConnect\ECAgent.exe"),
            };

            foreach (var defPath in defaultPaths)
            {
                if (File.Exists(defPath)) return defPath;
            }

            // 3. 遍历注册表（LocalMachine 与 CurrentUser，支持 32/64 位视图）
            string[] uninstallKeys =
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            };

            RegistryHive[] hives = { RegistryHive.LocalMachine, RegistryHive.CurrentUser };

            foreach (var hive in hives)
            {
                foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
                {
                    try
                    {
                        using var baseKey = RegistryKey.OpenBaseKey(hive, view);
                        foreach (var unKey in uninstallKeys)
                        {
                            using var subKeys = baseKey.OpenSubKey(unKey);
                            if (subKeys == null) continue;

                            foreach (var subName in subKeys.GetSubKeyNames())
                            {
                                using var appKey = subKeys.OpenSubKey(subName);
                                if (appKey == null) continue;

                                string? displayName = appKey.GetValue("DisplayName") as string;
                                if (!string.IsNullOrEmpty(displayName) &&
                                    (displayName.Contains("EasyConnect", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("Easy Connect", StringComparison.OrdinalIgnoreCase) ||
                                     displayName.Contains("深信服", StringComparison.OrdinalIgnoreCase)))
                                {
                                    string? rawPath = (appKey.GetValue("DisplayIcon") as string)
                                                   ?? (appKey.GetValue("InstallLocation") as string);

                                    string? cleaned = CleanExecutablePath(rawPath);
                                    if (!string.IsNullOrEmpty(cleaned) && File.Exists(cleaned))
                                    {
                                        return cleaned;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            return null;
        }

        private static string? CleanExecutablePath(string? rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath)) return null;

            string cleaned = rawPath.Trim().Trim('"');

            // 剥离可能存在的图标索引，例如 "C:\...\EasyConnect.exe,0"
            int commaIndex = cleaned.IndexOf(',');
            if (commaIndex > 0)
            {
                cleaned = cleaned.Substring(0, commaIndex).Trim().Trim('"');
            }

            if (File.Exists(cleaned))
            {
                if (cleaned.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    return cleaned;
            }
            else if (Directory.Exists(cleaned))
            {
                string candidate = Path.Combine(cleaned, "EasyConnect.exe");
                if (File.Exists(candidate)) return candidate;
            }

            return null;
        }

        public void Dispose()
        {
            StopKeepAlive();
        }
    }
}
