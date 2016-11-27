namespace CallListUpdateTool
{
    partial class Login
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
            this.BTNSelectFile = new System.Windows.Forms.Button();
            this.LabelFilePath = new System.Windows.Forms.Label();
            this.SGVListShow = new System.Windows.Forms.DataGridView();
            this.LLTMPDownload = new System.Windows.Forms.LinkLabel();
            this.UserPhoneNumber = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BTNReset = new System.Windows.Forms.Button();
            this.BTNUpload = new System.Windows.Forms.Button();
            this.LRumMessage = new System.Windows.Forms.Label();
            this.PBUpdateList = new System.Windows.Forms.ProgressBar();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.SGVListShow)).BeginInit();
            this.SuspendLayout();
            // 
            // BTNSelectFile
            // 
            this.BTNSelectFile.Font = new System.Drawing.Font("宋体", 15F);
            this.BTNSelectFile.Location = new System.Drawing.Point(312, 27);
            this.BTNSelectFile.Name = "BTNSelectFile";
            this.BTNSelectFile.Size = new System.Drawing.Size(115, 28);
            this.BTNSelectFile.TabIndex = 2;
            this.BTNSelectFile.Text = "选择文件";
            this.BTNSelectFile.UseVisualStyleBackColor = true;
            this.BTNSelectFile.Click += new System.EventHandler(this.BTNSelectFile_Click);
            // 
            // LabelFilePath
            // 
            this.LabelFilePath.AutoSize = true;
            this.LabelFilePath.Font = new System.Drawing.Font("宋体", 12F);
            this.LabelFilePath.Location = new System.Drawing.Point(23, 73);
            this.LabelFilePath.Name = "LabelFilePath";
            this.LabelFilePath.Size = new System.Drawing.Size(264, 16);
            this.LabelFilePath.TabIndex = 1;
            this.LabelFilePath.Text = "请安装MS Office 2007以上版本软件";
            // 
            // SGVListShow
            // 
            this.SGVListShow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SGVListShow.Location = new System.Drawing.Point(3, 100);
            this.SGVListShow.Name = "SGVListShow";
            this.SGVListShow.RowTemplate.Height = 23;
            this.SGVListShow.Size = new System.Drawing.Size(1257, 511);
            this.SGVListShow.TabIndex = 3;
            // 
            // LLTMPDownload
            // 
            this.LLTMPDownload.AutoSize = true;
            this.LLTMPDownload.Font = new System.Drawing.Font("宋体", 15F);
            this.LLTMPDownload.Location = new System.Drawing.Point(1009, 5);
            this.LLTMPDownload.Name = "LLTMPDownload";
            this.LLTMPDownload.Size = new System.Drawing.Size(89, 20);
            this.LLTMPDownload.TabIndex = 4;
            this.LLTMPDownload.TabStop = true;
            this.LLTMPDownload.Text = "模板下载";
            this.LLTMPDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LLTMPDownload_LinkClicked);
            // 
            // UserPhoneNumber
            // 
            this.UserPhoneNumber.Font = new System.Drawing.Font("宋体", 15F);
            this.UserPhoneNumber.FormattingEnabled = true;
            this.UserPhoneNumber.Location = new System.Drawing.Point(131, 27);
            this.UserPhoneNumber.Name = "UserPhoneNumber";
            this.UserPhoneNumber.Size = new System.Drawing.Size(163, 28);
            this.UserPhoneNumber.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F);
            this.label1.Location = new System.Drawing.Point(21, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "账单号码:";
            // 
            // BTNReset
            // 
            this.BTNReset.Font = new System.Drawing.Font("宋体", 15F);
            this.BTNReset.Location = new System.Drawing.Point(593, 27);
            this.BTNReset.Name = "BTNReset";
            this.BTNReset.Size = new System.Drawing.Size(85, 28);
            this.BTNReset.TabIndex = 7;
            this.BTNReset.Text = "重置";
            this.BTNReset.UseVisualStyleBackColor = true;
            this.BTNReset.Click += new System.EventHandler(this.BTNReset_Click);
            // 
            // BTNUpload
            // 
            this.BTNUpload.Font = new System.Drawing.Font("宋体", 15F);
            this.BTNUpload.Location = new System.Drawing.Point(453, 27);
            this.BTNUpload.Name = "BTNUpload";
            this.BTNUpload.Size = new System.Drawing.Size(116, 28);
            this.BTNUpload.TabIndex = 8;
            this.BTNUpload.Text = "确认上传";
            this.BTNUpload.UseVisualStyleBackColor = true;
            this.BTNUpload.Click += new System.EventHandler(this.BTNUpload_Click);
            // 
            // LRumMessage
            // 
            this.LRumMessage.AutoSize = true;
            this.LRumMessage.Font = new System.Drawing.Font("宋体", 12F);
            this.LRumMessage.Location = new System.Drawing.Point(707, 73);
            this.LRumMessage.Name = "LRumMessage";
            this.LRumMessage.Size = new System.Drawing.Size(120, 16);
            this.LRumMessage.TabIndex = 10;
            this.LRumMessage.Text = "请选择通话详单";
            // 
            // PBUpdateList
            // 
            this.PBUpdateList.Location = new System.Drawing.Point(700, 28);
            this.PBUpdateList.Name = "PBUpdateList";
            this.PBUpdateList.Size = new System.Drawing.Size(466, 23);
            this.PBUpdateList.TabIndex = 11;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("宋体", 12F);
            this.linkLabel1.Location = new System.Drawing.Point(1104, 8);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(72, 16);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "上传统计";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(874, 8);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(119, 12);
            this.linkLabel3.TabIndex = 14;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "MS ACE 12.0插件下载";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 636);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.SGVListShow);
            this.Controls.Add(this.PBUpdateList);
            this.Controls.Add(this.LRumMessage);
            this.Controls.Add(this.BTNUpload);
            this.Controls.Add(this.BTNReset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UserPhoneNumber);
            this.Controls.Add(this.LLTMPDownload);
            this.Controls.Add(this.LabelFilePath);
            this.Controls.Add(this.BTNSelectFile);
            this.Name = "Login";
            this.Text = "海汇软件通话记录上传工具";
            ((System.ComponentModel.ISupportInitialize)(this.SGVListShow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTNSelectFile;
        private System.Windows.Forms.Label LabelFilePath;
        private System.Windows.Forms.DataGridView SGVListShow;
        private System.Windows.Forms.LinkLabel LLTMPDownload;
        private System.Windows.Forms.ComboBox UserPhoneNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BTNReset;
        private System.Windows.Forms.Button BTNUpload;
        private System.Windows.Forms.Label LRumMessage;
        private System.Windows.Forms.ProgressBar PBUpdateList;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel3;
    }
}

