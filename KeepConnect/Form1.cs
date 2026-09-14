using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyConnectKeeper
{
    public partial class Form1 : Form
    {
        private readonly VpnService _vpnService = new VpnService();
        private readonly LogService _logService;
        private bool _isClosing = false;
        private bool _lastConnectivity = false;

        public Form1()
        {
            InitializeComponent();
            _logService = new LogService(AppendLog);
            _vpnService.OnLog += msg => _logService.Info(msg);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _logService.Info("程序已启动");
            UpdateStatusLabel("正在检查 VPN 状态...");
            _ = CheckStatus();
        }

        private async void timerStatus_Tick(object sender, EventArgs e)
        {
            await CheckStatus();
        }

        private async Task CheckStatus()
        {
            if (!_vpnService.IsKeepAliveActive)
            {
                _vpnService.Host = textBoxHost.Text.Trim();
                _vpnService.Port = (int)numericUpDownPort.Value;
                _vpnService.KeepAliveIntervalSeconds = (int)numericUpDownCycle.Value * 60;
            }

            bool isRunning = _vpnService.IsEasyConnectRunning();
            var (isConnected, method) = await _vpnService.CheckConnectivityWithDetailAsync();

            // Log connectivity changes
            if (isConnected != _lastConnectivity)
            {
                if (isConnected)
                    _logService.Info($"检测到内网已联通 [{method}]");
                else if (_lastConnectivity)
                    _logService.Info("检测到内网连接已断开");
                
                _lastConnectivity = isConnected;
            }

            if (_vpnService.IsKeepAliveActive)
            {
                UpdateStatusLabel($"保持连接活跃中 ({_vpnService.Host}:{_vpnService.Port})", Color.BlueViolet);
                buttonKeepAlive.Enabled = false;
                buttonStop.Enabled = true;
                groupBoxConfig.Enabled = false;
            }
            else
            {
                // 重点：始终允许用户手动点击开启保活，解除死锁限制
                buttonKeepAlive.Enabled = true;
                buttonStop.Enabled = false;
                groupBoxConfig.Enabled = true;

                if (isConnected)
                {
                    UpdateStatusLabel($"内网已联通 [{method}]", Color.Green);
                }
                else if (isRunning)
                {
                    UpdateStatusLabel("EasyConnect 运行中 (内网未联通)", Color.Orange);
                }
                else
                {
                    UpdateStatusLabel("未检测到 EasyConnect 进程", Color.Gray);
                }
            }
        }

        private void UpdateStatusLabel(string text, Color? color = null)
        {
            toolStripStatusLabel1.Text = $"状态：{text}";
            if (color.HasValue)
                toolStripStatusLabel1.ForeColor = color.Value;
            else
                toolStripStatusLabel1.ForeColor = SystemColors.ControlText;
        }

        private void AppendLog(string message)
        {
            if (richTextBoxLog.IsDisposed) return;

            if (richTextBoxLog.InvokeRequired)
            {
                try
                {
                    richTextBoxLog.Invoke(new Action<string>(AppendLog), message);
                }
                catch (ObjectDisposedException) { }
                return;
            }

            richTextBoxLog.AppendText(message + Environment.NewLine);
            richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
            richTextBoxLog.ScrollToCaret();

            // Limit log size in UI
            if (richTextBoxLog.Lines.Length > 1000)
            {
                richTextBoxLog.Select(0, richTextBoxLog.GetFirstCharIndexFromLine(100));
                richTextBoxLog.SelectedText = "";
            }
        }

        private void buttonRunEC_Click(object sender, EventArgs e)
        {
            string? path = _vpnService.GetEasyConnectInstallPath();
            if (string.IsNullOrEmpty(path))
            {
                _logService.Error("未找到 EasyConnect 安装路径");
                MessageBox.Show("未找到 EasyConnect 安装路径。请手动启动或确认是否已安装。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _logService.Info($"尝试启动 EasyConnect: {path}");
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                _logService.Error("启动 EasyConnect 失败", ex);
                MessageBox.Show($"启动失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonKeepAlive_Click(object sender, EventArgs e)
        {
            try
            {
                _vpnService.Host = textBoxHost.Text.Trim();
                _vpnService.Port = (int)numericUpDownPort.Value;
                _vpnService.KeepAliveIntervalSeconds = (int)numericUpDownCycle.Value * 60;

                _logService.Info($"开启保持连接：{_vpnService.Host}:{_vpnService.Port}，心跳周期：{numericUpDownCycle.Value} 分钟");
                await _vpnService.StartKeepAliveAsync();
                await CheckStatus();
            }
            catch (Exception ex)
            {
                _logService.Error("开启保持连接失败", ex);
                MessageBox.Show($"开启失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonStop_Click(object sender, EventArgs e)
        {
            _logService.Info("停止保持连接");
            _vpnService.StopKeepAlive();
            await CheckStatus();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreFromTray();
        }

        private void toolStripMenuItemRestore_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void RestoreFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
            notifyIcon1.Visible = true;
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            _logService.Info("正在退出程序...");
            _isClosing = true;
            try
            {
                notifyIcon1.Visible = false;
                notifyIcon1.Dispose();
            }
            catch { }
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isClosing && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                notifyIcon1.Visible = true;
                _logService.Info("程序已最小化到系统托盘");
            }
            else
            {
                try
                {
                    notifyIcon1.Visible = false;
                    notifyIcon1.Dispose();
                }
                catch { }
                _vpnService.Dispose();
            }
        }
    }
}
