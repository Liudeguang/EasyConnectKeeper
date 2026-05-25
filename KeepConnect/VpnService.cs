using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace EasyConnectKeeper
{
    public class VpnService : IDisposable
    {
        private TcpClient? _keepAliveClient;
        private readonly string _clientName = "EasyConnect";
        
        public string Host { get; set; } = "10.0.0.1";
        public int Port { get; set; } = 80;
        public int KeepAliveIntervalSeconds { get; set; } = 600; // 10 minutes default

        public bool IsKeepAliveActive => _keepAliveClient?.Connected ?? false;

        public async Task<bool> CheckConnectivityAsync()
        {
            using var client = new TcpClient();
            try
            {
                var connectTask = client.ConnectAsync(Host, Port);
                var delayTask = Task.Delay(3000); // 3 seconds timeout

                var completedTask = await Task.WhenAny(connectTask, delayTask);
                return completedTask == connectTask && client.Connected;
            }
            catch
            {
                return false;
            }
        }

        public void StartKeepAlive()
        {
            StopKeepAlive();

            try
            {
                _keepAliveClient = new TcpClient();
                _keepAliveClient.Connect(Host, Port);

                var socket = _keepAliveClient.Client;
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                
                // Windows specific keep-alive settings
                // TcpKeepAliveTime: time before first keep-alive probe (ms)
                // TcpKeepAliveInterval: time between probes (ms)
                // TcpKeepAliveRetryCount: number of probes before disconnect
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, KeepAliveIntervalSeconds * 1000);
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 3000); // 3 seconds
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 5);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start Keep-Alive: {ex.Message}");
                throw;
            }
        }

        public void StopKeepAlive()
        {
            _keepAliveClient?.Dispose();
            _keepAliveClient = null;
        }

        public bool IsEasyConnectRunning()
        {
            return Process.GetProcessesByName(_clientName).Length > 0;
        }

        public string? GetEasyConnectInstallPath()
        {
            string[] uninstallKeys =
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            };

            foreach (string uninstallKey in uninstallKeys)
            {
                using var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                using var key = hklm.OpenSubKey(uninstallKey);
                if (key == null) continue;

                foreach (string subKeyName in key.GetSubKeyNames())
                {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey == null) continue;

                    string? name = subKey.GetValue("DisplayName") as string;
                    if (name != null && name.Equals(_clientName, StringComparison.OrdinalIgnoreCase))
                    {
                        return subKey.GetValue("DisplayIcon") as string ?? subKey.GetValue("InstallLocation") as string;
                    }
                }
            }
            return null;
        }

        public void Dispose()
        {
            StopKeepAlive();
        }
    }
}
