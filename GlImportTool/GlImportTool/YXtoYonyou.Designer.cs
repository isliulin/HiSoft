namespace GlImportTool
{
    partial class YXtoYonyou
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
            this.TBMDBPath = new System.Windows.Forms.TextBox();
            this.BTNOpenMDB = new System.Windows.Forms.Button();
            this.LMessage = new System.Windows.Forms.Label();
            this.BTNLoadGLAcc = new System.Windows.Forms.Button();
            this.DGVShow = new System.Windows.Forms.DataGridView();
            this.BTNRebuildAcc = new System.Windows.Forms.Button();
            this.BTNSaveAcc = new System.Windows.Forms.Button();
            this.BTNImportVouch = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.BTNTImpHand = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGVShow)).BeginInit();
            this.SuspendLayout();
            // 
            // TBMDBPath
            // 
            this.TBMDBPath.Location = new System.Drawing.Point(3, 6);
            this.TBMDBPath.Name = "TBMDBPath";
            this.TBMDBPath.Size = new System.Drawing.Size(224, 21);
            this.TBMDBPath.TabIndex = 0;
            // 
            // BTNOpenMDB
            // 
            this.BTNOpenMDB.Location = new System.Drawing.Point(234, 6);
            this.BTNOpenMDB.Name = "BTNOpenMDB";
            this.BTNOpenMDB.Size = new System.Drawing.Size(69, 23);
            this.BTNOpenMDB.TabIndex = 1;
            this.BTNOpenMDB.Text = "打开文件";
            this.BTNOpenMDB.UseVisualStyleBackColor = true;
            this.BTNOpenMDB.Click += new System.EventHandler(this.BTNOpenMDB_Click);
            // 
            // LMessage
            // 
            this.LMessage.AutoSize = true;
            this.LMessage.Location = new System.Drawing.Point(319, 14);
            this.LMessage.Name = "LMessage";
            this.LMessage.Size = new System.Drawing.Size(149, 12);
            this.LMessage.TabIndex = 2;
            this.LMessage.Text = "请选择友信数据库所在位置";
            // 
            // BTNLoadGLAcc
            // 
            this.BTNLoadGLAcc.Enabled = false;
            this.BTNLoadGLAcc.Location = new System.Drawing.Point(3, 34);
            this.BTNLoadGLAcc.Name = "BTNLoadGLAcc";
            this.BTNLoadGLAcc.Size = new System.Drawing.Size(75, 23);
            this.BTNLoadGLAcc.TabIndex = 3;
            this.BTNLoadGLAcc.Text = "读取会计科目";
            this.BTNLoadGLAcc.UseVisualStyleBackColor = true;
            this.BTNLoadGLAcc.Click += new System.EventHandler(this.BTNLoadGLAcc_Click);
            // 
            // DGVShow
            // 
            this.DGVShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGVShow.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.DGVShow.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.DGVShow.Location = new System.Drawing.Point(3, 64);
            this.DGVShow.Name = "DGVShow";
            this.DGVShow.RowTemplate.Height = 23;
            this.DGVShow.Size = new System.Drawing.Size(981, 398);
            this.DGVShow.TabIndex = 4;
            // 
            // BTNRebuildAcc
            // 
            this.BTNRebuildAcc.Enabled = false;
            this.BTNRebuildAcc.Location = new System.Drawing.Point(97, 34);
            this.BTNRebuildAcc.Name = "BTNRebuildAcc";
            this.BTNRebuildAcc.Size = new System.Drawing.Size(75, 23);
            this.BTNRebuildAcc.TabIndex = 5;
            this.BTNRebuildAcc.Text = "重建科目";
            this.BTNRebuildAcc.UseVisualStyleBackColor = true;
            this.BTNRebuildAcc.Click += new System.EventHandler(this.BTNRebuildAcc_Click);
            // 
            // BTNSaveAcc
            // 
            this.BTNSaveAcc.Enabled = false;
            this.BTNSaveAcc.Location = new System.Drawing.Point(190, 33);
            this.BTNSaveAcc.Name = "BTNSaveAcc";
            this.BTNSaveAcc.Size = new System.Drawing.Size(93, 23);
            this.BTNSaveAcc.TabIndex = 6;
            this.BTNSaveAcc.Text = "保存为T+模板";
            this.BTNSaveAcc.UseVisualStyleBackColor = true;
            this.BTNSaveAcc.Click += new System.EventHandler(this.BTNSaveAcc_Click);
            // 
            // BTNImportVouch
            // 
            this.BTNImportVouch.Enabled = false;
            this.BTNImportVouch.Location = new System.Drawing.Point(303, 34);
            this.BTNImportVouch.Name = "BTNImportVouch";
            this.BTNImportVouch.Size = new System.Drawing.Size(75, 23);
            this.BTNImportVouch.TabIndex = 7;
            this.BTNImportVouch.Text = "导入凭证";
            this.BTNImportVouch.UseVisualStyleBackColor = true;
            this.BTNImportVouch.Click += new System.EventHandler(this.BTNImportVouch_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(870, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 42);
            this.button1.TabIndex = 8;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BTNTImpHand
            // 
            this.BTNTImpHand.Location = new System.Drawing.Point(392, 34);
            this.BTNTImpHand.Name = "BTNTImpHand";
            this.BTNTImpHand.Size = new System.Drawing.Size(75, 23);
            this.BTNTImpHand.TabIndex = 9;
            this.BTNTImpHand.Text = "转T+处理";
            this.BTNTImpHand.UseVisualStyleBackColor = true;
            this.BTNTImpHand.Click += new System.EventHandler(this.BTNTImpHand_Click);
            // 
            // YXtoYonyou
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(986, 474);
            this.Controls.Add(this.BTNTImpHand);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BTNImportVouch);
            this.Controls.Add(this.BTNSaveAcc);
            this.Controls.Add(this.BTNRebuildAcc);
            this.Controls.Add(this.DGVShow);
            this.Controls.Add(this.BTNLoadGLAcc);
            this.Controls.Add(this.LMessage);
            this.Controls.Add(this.BTNOpenMDB);
            this.Controls.Add(this.TBMDBPath);
            this.Name = "YXtoYonyou";
            this.Text = "友信转用友工具";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.DGVShow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TBMDBPath;
        private System.Windows.Forms.Button BTNOpenMDB;
        private System.Windows.Forms.Label LMessage;
        private System.Windows.Forms.Button BTNLoadGLAcc;
        private System.Windows.Forms.DataGridView DGVShow;
        private System.Windows.Forms.Button BTNRebuildAcc;
        private System.Windows.Forms.Button BTNSaveAcc;
        private System.Windows.Forms.Button BTNImportVouch;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button BTNTImpHand;
    }
}

