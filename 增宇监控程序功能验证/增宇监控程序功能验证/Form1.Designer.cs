namespace 增宇监控程序功能验证
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.RunMSG = new System.Windows.Forms.Label();
            this.RCK = new System.ComponentModel.BackgroundWorker();
            this.WarringTXT = new System.Windows.Forms.Label();
            this.WindowsList = new System.Windows.Forms.ListBox();
            this.LastRunTimeMSG = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BTNRestartNow = new System.Windows.Forms.Button();
            this.LBTimerMSG = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // RunMSG
            // 
            this.RunMSG.AutoSize = true;
            this.RunMSG.Font = new System.Drawing.Font("宋体", 20F);
            this.RunMSG.Location = new System.Drawing.Point(21, 57);
            this.RunMSG.Name = "RunMSG";
            this.RunMSG.Size = new System.Drawing.Size(96, 27);
            this.RunMSG.TabIndex = 0;
            this.RunMSG.Text = "label1";
            // 
            // RCK
            // 
            this.RCK.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RCK_DoWork);
            this.RCK.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.RCK_ProgressChanged);
            this.RCK.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.RCK_RunWorkerCompleted);
            // 
            // WarringTXT
            // 
            this.WarringTXT.AutoSize = true;
            this.WarringTXT.Font = new System.Drawing.Font("宋体", 20F);
            this.WarringTXT.Location = new System.Drawing.Point(12, 19);
            this.WarringTXT.Name = "WarringTXT";
            this.WarringTXT.Size = new System.Drawing.Size(363, 27);
            this.WarringTXT.TabIndex = 1;
            this.WarringTXT.Text = "增宇语音服务监控，勿动！！";
            // 
            // WindowsList
            // 
            this.WindowsList.FormattingEnabled = true;
            this.WindowsList.ItemHeight = 12;
            this.WindowsList.Location = new System.Drawing.Point(12, 130);
            this.WindowsList.Name = "WindowsList";
            this.WindowsList.Size = new System.Drawing.Size(402, 196);
            this.WindowsList.TabIndex = 2;
            // 
            // LastRunTimeMSG
            // 
            this.LastRunTimeMSG.AutoSize = true;
            this.LastRunTimeMSG.Location = new System.Drawing.Point(112, 99);
            this.LastRunTimeMSG.Name = "LastRunTimeMSG";
            this.LastRunTimeMSG.Size = new System.Drawing.Size(41, 12);
            this.LastRunTimeMSG.TabIndex = 3;
            this.LastRunTimeMSG.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "上次运行时间";
            // 
            // BTNRestartNow
            // 
            this.BTNRestartNow.Location = new System.Drawing.Point(439, 12);
            this.BTNRestartNow.Name = "BTNRestartNow";
            this.BTNRestartNow.Size = new System.Drawing.Size(117, 57);
            this.BTNRestartNow.TabIndex = 5;
            this.BTNRestartNow.Text = "立即重启语音服务";
            this.BTNRestartNow.UseVisualStyleBackColor = true;
            this.BTNRestartNow.Click += new System.EventHandler(this.BTNRestartNow_Click);
            // 
            // LBTimerMSG
            // 
            this.LBTimerMSG.FormattingEnabled = true;
            this.LBTimerMSG.ItemHeight = 12;
            this.LBTimerMSG.Location = new System.Drawing.Point(420, 130);
            this.LBTimerMSG.Name = "LBTimerMSG";
            this.LBTimerMSG.Size = new System.Drawing.Size(139, 196);
            this.LBTimerMSG.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 338);
            this.Controls.Add(this.LBTimerMSG);
            this.Controls.Add(this.BTNRestartNow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LastRunTimeMSG);
            this.Controls.Add(this.WindowsList);
            this.Controls.Add(this.WarringTXT);
            this.Controls.Add(this.RunMSG);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label RunMSG;
        private System.ComponentModel.BackgroundWorker RCK;
        private System.Windows.Forms.Label WarringTXT;
        private System.Windows.Forms.ListBox WindowsList;
        private System.Windows.Forms.Label LastRunTimeMSG;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BTNRestartNow;
        private System.Windows.Forms.ListBox LBTimerMSG;
    }
}

