namespace EasyConnectKeeper
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panelHeader = new Panel();
            labelTitle = new Label();
            groupBoxConfig = new GroupBox();
            labelCycle = new Label();
            numericUpDownCycle = new NumericUpDown();
            numericUpDownPort = new NumericUpDown();
            labelPort = new Label();
            textBoxHost = new TextBox();
            labelHost = new Label();
            buttonRunEC = new Button();
            buttonKeepAlive = new Button();
            buttonStop = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStripTray = new ContextMenuStrip(components);
            toolStripMenuItemRestore = new ToolStripMenuItem();
            toolStripMenuItemExit = new ToolStripMenuItem();
            timerStatus = new System.Windows.Forms.Timer(components);
            richTextBoxLog = new RichTextBox();
            panelHeader.SuspendLayout();
            groupBoxConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCycle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownPort).BeginInit();
            statusStrip1.SuspendLayout();
            contextMenuStripTray.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(0, 122, 204);
            panelHeader.Controls.Add(labelTitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(600, 80);
            panelHeader.TabIndex = 0;
            // 
            // labelTitle
            // 
            labelTitle.Dock = DockStyle.Fill;
            labelTitle.Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            labelTitle.ForeColor = Color.White;
            labelTitle.Location = new Point(0, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(600, 80);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "EasyConnectKeeper 自动联通程序";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBoxConfig
            // 
            groupBoxConfig.Controls.Add(labelCycle);
            groupBoxConfig.Controls.Add(numericUpDownCycle);
            groupBoxConfig.Controls.Add(numericUpDownPort);
            groupBoxConfig.Controls.Add(labelPort);
            groupBoxConfig.Controls.Add(textBoxHost);
            groupBoxConfig.Controls.Add(labelHost);
            groupBoxConfig.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            groupBoxConfig.Location = new Point(20, 100);
            groupBoxConfig.Name = "groupBoxConfig";
            groupBoxConfig.Size = new Size(560, 150);
            groupBoxConfig.TabIndex = 1;
            groupBoxConfig.TabStop = false;
            groupBoxConfig.Text = "配置选项";
            // 
            // labelCycle
            // 
            labelCycle.AutoSize = true;
            labelCycle.Location = new Point(320, 95);
            labelCycle.Name = "labelCycle";
            labelCycle.Size = new Size(116, 24);
            labelCycle.TabIndex = 5;
            labelCycle.Text = "周期 (分钟)：";
            // 
            // numericUpDownCycle
            // 
            numericUpDownCycle.Location = new Point(440, 92);
            numericUpDownCycle.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            numericUpDownCycle.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownCycle.Name = "numericUpDownCycle";
            numericUpDownCycle.Size = new Size(100, 31);
            numericUpDownCycle.TabIndex = 4;
            numericUpDownCycle.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // numericUpDownPort
            // 
            numericUpDownPort.Location = new Point(100, 92);
            numericUpDownPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDownPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownPort.Name = "numericUpDownPort";
            numericUpDownPort.Size = new Size(150, 31);
            numericUpDownPort.TabIndex = 3;
            numericUpDownPort.Value = new decimal(new int[] { 80, 0, 0, 0 });
            // 
            // labelPort
            // 
            labelPort.AutoSize = true;
            labelPort.Location = new Point(20, 95);
            labelPort.Name = "labelPort";
            labelPort.Size = new Size(64, 24);
            labelPort.TabIndex = 2;
            labelPort.Text = "端口：";
            // 
            // textBoxHost
            // 
            textBoxHost.Location = new Point(100, 40);
            textBoxHost.Name = "textBoxHost";
            textBoxHost.Size = new Size(440, 31);
            textBoxHost.TabIndex = 1;
            textBoxHost.Text = "10.0.0.1";
            // 
            // labelHost
            // 
            labelHost.AutoSize = true;
            labelHost.Location = new Point(20, 43);
            labelHost.Name = "labelHost";
            labelHost.Size = new Size(66, 24);
            labelHost.TabIndex = 0;
            labelHost.Text = "主机：";
            // 
            // buttonRunEC
            // 
            buttonRunEC.FlatStyle = FlatStyle.Flat;
            buttonRunEC.Location = new Point(20, 270);
            buttonRunEC.Name = "buttonRunEC";
            buttonRunEC.Size = new Size(180, 50);
            buttonRunEC.TabIndex = 2;
            buttonRunEC.Text = "启动客户端";
            buttonRunEC.UseVisualStyleBackColor = true;
            buttonRunEC.Click += buttonRunEC_Click;
            // 
            // buttonKeepAlive
            // 
            buttonKeepAlive.BackColor = Color.LightGreen;
            buttonKeepAlive.FlatStyle = FlatStyle.Flat;
            buttonKeepAlive.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonKeepAlive.Location = new Point(210, 270);
            buttonKeepAlive.Name = "buttonKeepAlive";
            buttonKeepAlive.Size = new Size(180, 50);
            buttonKeepAlive.TabIndex = 3;
            buttonKeepAlive.Text = "开启保持连接";
            buttonKeepAlive.UseVisualStyleBackColor = false;
            buttonKeepAlive.Click += buttonKeepAlive_Click;
            // 
            // buttonStop
            // 
            buttonStop.BackColor = Color.Salmon;
            buttonStop.Enabled = false;
            buttonStop.FlatStyle = FlatStyle.Flat;
            buttonStop.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            buttonStop.Location = new Point(400, 270);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(180, 50);
            buttonStop.TabIndex = 4;
            buttonStop.Text = "停止保持";
            buttonStop.UseVisualStyleBackColor = false;
            buttonStop.Click += buttonStop_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 568);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(600, 32);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(82, 25);
            toolStripStatusLabel1.Text = "状态：就绪";
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStripTray;
            notifyIcon1.Icon = (Icon)resources.GetObject("$this.Icon");
            notifyIcon1.Text = "EasyConnectKeeper";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // contextMenuStripTray
            // 
            contextMenuStripTray.ImageScalingSize = new Size(24, 24);
            contextMenuStripTray.Items.AddRange(new ToolStripItem[] { toolStripMenuItemRestore, toolStripMenuItemExit });
            contextMenuStripTray.Name = "contextMenuStripTray";
            contextMenuStripTray.Size = new Size(117, 68);
            // 
            // toolStripMenuItemRestore
            // 
            toolStripMenuItemRestore.Name = "toolStripMenuItemRestore";
            toolStripMenuItemRestore.Size = new Size(116, 32);
            toolStripMenuItemRestore.Text = "恢复";
            toolStripMenuItemRestore.Click += toolStripMenuItemRestore_Click;
            // 
            // toolStripMenuItemExit
            // 
            toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            toolStripMenuItemExit.Size = new Size(116, 32);
            toolStripMenuItemExit.Text = "退出";
            toolStripMenuItemExit.Click += toolStripMenuItemExit_Click;
            // 
            // timerStatus
            // 
            timerStatus.Enabled = true;
            timerStatus.Interval = 5000;
            timerStatus.Tick += timerStatus_Tick;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.BackColor = Color.FromArgb(30, 30, 30);
            richTextBoxLog.BorderStyle = BorderStyle.None;
            richTextBoxLog.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBoxLog.ForeColor = Color.LightGray;
            richTextBoxLog.Location = new Point(20, 340);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.ReadOnly = true;
            richTextBoxLog.Size = new Size(560, 200);
            richTextBoxLog.TabIndex = 6;
            richTextBoxLog.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 600);
            Controls.Add(richTextBoxLog);
            Controls.Add(statusStrip1);
            Controls.Add(buttonStop);
            Controls.Add(buttonKeepAlive);
            Controls.Add(buttonRunEC);
            Controls.Add(groupBoxConfig);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EasyConnectKeeper v1.1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            Resize += Form1_Resize;
            panelHeader.ResumeLayout(false);
            groupBoxConfig.ResumeLayout(false);
            groupBoxConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCycle).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownPort).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenuStripTray.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel panelHeader;
        private Label labelTitle;
        private GroupBox groupBoxConfig;
        private Label labelCycle;
        private NumericUpDown numericUpDownCycle;
        private NumericUpDown numericUpDownPort;
        private Label labelPort;
        private TextBox textBoxHost;
        private Label labelHost;
        private Button buttonRunEC;
        private Button buttonKeepAlive;
        private Button buttonStop;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStripTray;
        private ToolStripMenuItem toolStripMenuItemRestore;
        private ToolStripMenuItem toolStripMenuItemExit;
        private System.Windows.Forms.Timer timerStatus;
        private RichTextBox richTextBoxLog;
    }
}
