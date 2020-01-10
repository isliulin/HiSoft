namespace GlImportTool
{
    partial class YXImportVouch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.TBTServer = new System.Windows.Forms.TextBox();
            this.BTNLinkT = new System.Windows.Forms.Button();
            this.TBTUserName = new System.Windows.Forms.TextBox();
            this.TBTUserPwd = new System.Windows.Forms.TextBox();
            this.TBTZTNum = new System.Windows.Forms.TextBox();
            this.BTNTLogin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.RTBMsg = new System.Windows.Forms.RichTextBox();
            this.BTNTlogout = new System.Windows.Forms.Button();
            this.DGVMain = new System.Windows.Forms.DataGridView();
            this.BTNBuildQC = new System.Windows.Forms.Button();
            this.CBAccYM = new System.Windows.Forms.ComboBox();
            this.BTNImportAcc = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.TBExtCode = new System.Windows.Forms.TextBox();
            this.TBDocType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TBVouDate = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.TBVchMemo = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.TBAttVouNum = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.PBImpVch = new System.Windows.Forms.ProgressBar();
            this.LImportInfo = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.TBlogDate = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGVMain)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.CausesValidation = false;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(17, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "http://";
            // 
            // TBTServer
            // 
            this.TBTServer.Location = new System.Drawing.Point(66, 25);
            this.TBTServer.Name = "TBTServer";
            this.TBTServer.Size = new System.Drawing.Size(233, 21);
            this.TBTServer.TabIndex = 1;
            // 
            // BTNLinkT
            // 
            this.BTNLinkT.Location = new System.Drawing.Point(311, 25);
            this.BTNLinkT.Name = "BTNLinkT";
            this.BTNLinkT.Size = new System.Drawing.Size(79, 23);
            this.BTNLinkT.TabIndex = 2;
            this.BTNLinkT.Text = "连接服务器";
            this.BTNLinkT.UseVisualStyleBackColor = true;
            this.BTNLinkT.Click += new System.EventHandler(this.BTNLinkTServer_Click);
            // 
            // TBTUserName
            // 
            this.TBTUserName.Location = new System.Drawing.Point(405, 26);
            this.TBTUserName.Name = "TBTUserName";
            this.TBTUserName.Size = new System.Drawing.Size(100, 21);
            this.TBTUserName.TabIndex = 3;
            // 
            // TBTUserPwd
            // 
            this.TBTUserPwd.Location = new System.Drawing.Point(524, 26);
            this.TBTUserPwd.Name = "TBTUserPwd";
            this.TBTUserPwd.Size = new System.Drawing.Size(100, 21);
            this.TBTUserPwd.TabIndex = 4;
            // 
            // TBTZTNum
            // 
            this.TBTZTNum.Location = new System.Drawing.Point(639, 26);
            this.TBTZTNum.Name = "TBTZTNum";
            this.TBTZTNum.Size = new System.Drawing.Size(71, 21);
            this.TBTZTNum.TabIndex = 5;
            // 
            // BTNTLogin
            // 
            this.BTNTLogin.Enabled = false;
            this.BTNTLogin.Location = new System.Drawing.Point(826, 24);
            this.BTNTLogin.Name = "BTNTLogin";
            this.BTNTLogin.Size = new System.Drawing.Size(75, 23);
            this.BTNTLogin.TabIndex = 6;
            this.BTNTLogin.Text = "登陆账套";
            this.BTNTLogin.UseVisualStyleBackColor = true;
            this.BTNTLogin.Click += new System.EventHandler(this.BTNTLogin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.CausesValidation = false;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(126, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "T Plus 服务器地址";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.CausesValidation = false;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(436, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "用户名";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.CausesValidation = false;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(558, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "密码";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.CausesValidation = false;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(648, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "账套编号";
            // 
            // RTBMsg
            // 
            this.RTBMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTBMsg.Location = new System.Drawing.Point(10, 406);
            this.RTBMsg.Name = "RTBMsg";
            this.RTBMsg.Size = new System.Drawing.Size(891, 73);
            this.RTBMsg.TabIndex = 11;
            this.RTBMsg.Text = "";
            // 
            // BTNTlogout
            // 
            this.BTNTlogout.Enabled = false;
            this.BTNTlogout.Location = new System.Drawing.Point(884, 1);
            this.BTNTlogout.Name = "BTNTlogout";
            this.BTNTlogout.Size = new System.Drawing.Size(17, 23);
            this.BTNTlogout.TabIndex = 12;
            this.BTNTlogout.Text = "注销";
            this.BTNTlogout.UseVisualStyleBackColor = true;
            this.BTNTlogout.Visible = false;
            this.BTNTlogout.Click += new System.EventHandler(this.BTNTlogout_Click);
            // 
            // DGVMain
            // 
            this.DGVMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGVMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVMain.Location = new System.Drawing.Point(10, 127);
            this.DGVMain.Name = "DGVMain";
            this.DGVMain.RowTemplate.Height = 23;
            this.DGVMain.Size = new System.Drawing.Size(891, 273);
            this.DGVMain.TabIndex = 13;
            // 
            // BTNBuildQC
            // 
            this.BTNBuildQC.Enabled = false;
            this.BTNBuildQC.Location = new System.Drawing.Point(13, 53);
            this.BTNBuildQC.Name = "BTNBuildQC";
            this.BTNBuildQC.Size = new System.Drawing.Size(88, 23);
            this.BTNBuildQC.TabIndex = 14;
            this.BTNBuildQC.Text = "生成期初余额";
            this.BTNBuildQC.UseVisualStyleBackColor = true;
            // 
            // CBAccYM
            // 
            this.CBAccYM.FormattingEnabled = true;
            this.CBAccYM.Location = new System.Drawing.Point(164, 56);
            this.CBAccYM.Name = "CBAccYM";
            this.CBAccYM.Size = new System.Drawing.Size(80, 20);
            this.CBAccYM.TabIndex = 15;
            // 
            // BTNImportAcc
            // 
            this.BTNImportAcc.Enabled = false;
            this.BTNImportAcc.Location = new System.Drawing.Point(258, 55);
            this.BTNImportAcc.Name = "BTNImportAcc";
            this.BTNImportAcc.Size = new System.Drawing.Size(70, 23);
            this.BTNImportAcc.TabIndex = 16;
            this.BTNImportAcc.Text = "导入凭证";
            this.BTNImportAcc.UseVisualStyleBackColor = true;
            this.BTNImportAcc.Click += new System.EventHandler(this.BTNLoadAcc_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.CausesValidation = false;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(108, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "会计期间";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.CausesValidation = false;
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(21, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 18;
            this.label7.Text = "凭证编号";
            // 
            // TBExtCode
            // 
            this.TBExtCode.Location = new System.Drawing.Point(76, 92);
            this.TBExtCode.Name = "TBExtCode";
            this.TBExtCode.Size = new System.Drawing.Size(100, 21);
            this.TBExtCode.TabIndex = 19;
            // 
            // TBDocType
            // 
            this.TBDocType.Location = new System.Drawing.Point(251, 92);
            this.TBDocType.Name = "TBDocType";
            this.TBDocType.Size = new System.Drawing.Size(100, 21);
            this.TBDocType.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.CausesValidation = false;
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(195, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 20;
            this.label8.Text = "凭证类别";
            // 
            // TBVouDate
            // 
            this.TBVouDate.Location = new System.Drawing.Point(431, 92);
            this.TBVouDate.Name = "TBVouDate";
            this.TBVouDate.Size = new System.Drawing.Size(100, 21);
            this.TBVouDate.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.CausesValidation = false;
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(375, 97);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "凭证日期";
            // 
            // TBVchMemo
            // 
            this.TBVchMemo.Location = new System.Drawing.Point(711, 92);
            this.TBVchMemo.Name = "TBVchMemo";
            this.TBVchMemo.Size = new System.Drawing.Size(190, 21);
            this.TBVchMemo.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.CausesValidation = false;
            this.label10.Enabled = false;
            this.label10.Location = new System.Drawing.Point(652, 97);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 24;
            this.label10.Text = "凭证备注";
            // 
            // TBAttVouNum
            // 
            this.TBAttVouNum.Location = new System.Drawing.Point(593, 92);
            this.TBAttVouNum.Name = "TBAttVouNum";
            this.TBAttVouNum.Size = new System.Drawing.Size(39, 21);
            this.TBAttVouNum.TabIndex = 27;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.CausesValidation = false;
            this.label11.Enabled = false;
            this.label11.Location = new System.Drawing.Point(537, 97);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 26;
            this.label11.Text = "附属单据";
            // 
            // PBImpVch
            // 
            this.PBImpVch.Location = new System.Drawing.Point(349, 56);
            this.PBImpVch.Name = "PBImpVch";
            this.PBImpVch.Size = new System.Drawing.Size(552, 23);
            this.PBImpVch.TabIndex = 28;
            // 
            // LImportInfo
            // 
            this.LImportInfo.AutoSize = true;
            this.LImportInfo.Location = new System.Drawing.Point(570, 61);
            this.LImportInfo.Name = "LImportInfo";
            this.LImportInfo.Size = new System.Drawing.Size(77, 12);
            this.LImportInfo.TabIndex = 29;
            this.LImportInfo.Text = "准备导入凭证";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.CausesValidation = false;
            this.label12.Enabled = false;
            this.label12.Location = new System.Drawing.Point(745, 8);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 31;
            this.label12.Text = "登陆日期";
            // 
            // TBlogDate
            // 
            this.TBlogDate.Location = new System.Drawing.Point(717, 26);
            this.TBlogDate.Name = "TBlogDate";
            this.TBlogDate.Size = new System.Drawing.Size(100, 21);
            this.TBlogDate.TabIndex = 30;
            // 
            // YXImportVouch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 482);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.TBlogDate);
            this.Controls.Add(this.LImportInfo);
            this.Controls.Add(this.PBImpVch);
            this.Controls.Add(this.TBAttVouNum);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.TBVchMemo);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.TBVouDate);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.TBDocType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TBExtCode);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BTNImportAcc);
            this.Controls.Add(this.CBAccYM);
            this.Controls.Add(this.BTNBuildQC);
            this.Controls.Add(this.DGVMain);
            this.Controls.Add(this.BTNTlogout);
            this.Controls.Add(this.RTBMsg);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BTNTLogin);
            this.Controls.Add(this.TBTZTNum);
            this.Controls.Add(this.TBTUserPwd);
            this.Controls.Add(this.TBTUserName);
            this.Controls.Add(this.BTNLinkT);
            this.Controls.Add(this.TBTServer);
            this.Controls.Add(this.label1);
            this.Name = "YXImportVouch";
            this.Text = "YXImportVouch";
            ((System.ComponentModel.ISupportInitialize)(this.DGVMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TBTServer;
        private System.Windows.Forms.Button BTNLinkT;
        private System.Windows.Forms.TextBox TBTUserName;
        private System.Windows.Forms.TextBox TBTUserPwd;
        private System.Windows.Forms.TextBox TBTZTNum;
        private System.Windows.Forms.Button BTNTLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox RTBMsg;
        private System.Windows.Forms.Button BTNTlogout;
        private System.Windows.Forms.DataGridView DGVMain;
        private System.Windows.Forms.Button BTNBuildQC;
        private System.Windows.Forms.ComboBox CBAccYM;
        private System.Windows.Forms.Button BTNImportAcc;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TBExtCode;
        private System.Windows.Forms.TextBox TBDocType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TBVouDate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TBVchMemo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox TBAttVouNum;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ProgressBar PBImpVch;
        private System.Windows.Forms.Label LImportInfo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox TBlogDate;
    }
}