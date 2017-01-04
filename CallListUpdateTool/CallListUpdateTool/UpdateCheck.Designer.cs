namespace CallListUpdateTool
{
    partial class UpdateCheck
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
            this.DGVCounterShow = new System.Windows.Forms.DataGridView();
            this.BTNLoading = new System.Windows.Forms.Button();
            this.LTitleNote = new System.Windows.Forms.Label();
            this.CBYear = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGVCounterShow)).BeginInit();
            this.SuspendLayout();
            // 
            // DGVCounterShow
            // 
            this.DGVCounterShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGVCounterShow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVCounterShow.Location = new System.Drawing.Point(0, 40);
            this.DGVCounterShow.Name = "DGVCounterShow";
            this.DGVCounterShow.RowTemplate.Height = 23;
            this.DGVCounterShow.Size = new System.Drawing.Size(744, 398);
            this.DGVCounterShow.TabIndex = 0;
            // 
            // BTNLoading
            // 
            this.BTNLoading.Location = new System.Drawing.Point(115, 7);
            this.BTNLoading.Name = "BTNLoading";
            this.BTNLoading.Size = new System.Drawing.Size(88, 23);
            this.BTNLoading.TabIndex = 1;
            this.BTNLoading.Text = "通话次数统计";
            this.BTNLoading.UseVisualStyleBackColor = true;
            this.BTNLoading.Click += new System.EventHandler(this.BTNLoading_Click);
            // 
            // LTitleNote
            // 
            this.LTitleNote.AutoSize = true;
            this.LTitleNote.Location = new System.Drawing.Point(214, 12);
            this.LTitleNote.Name = "LTitleNote";
            this.LTitleNote.Size = new System.Drawing.Size(185, 12);
            this.LTitleNote.TabIndex = 2;
            this.LTitleNote.Text = "最新年度通话次数统计，单位：次";
            // 
            // CBYear
            // 
            this.CBYear.FormattingEnabled = true;
            this.CBYear.Location = new System.Drawing.Point(12, 9);
            this.CBYear.Name = "CBYear";
            this.CBYear.Size = new System.Drawing.Size(90, 20);
            this.CBYear.TabIndex = 3;
            // 
            // UpdateCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 440);
            this.Controls.Add(this.CBYear);
            this.Controls.Add(this.LTitleNote);
            this.Controls.Add(this.BTNLoading);
            this.Controls.Add(this.DGVCounterShow);
            this.Name = "UpdateCheck";
            this.Text = "UpdateCheck";
            ((System.ComponentModel.ISupportInitialize)(this.DGVCounterShow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DGVCounterShow;
        private System.Windows.Forms.Button BTNLoading;
        private System.Windows.Forms.Label LTitleNote;
        private System.Windows.Forms.ComboBox CBYear;
    }
}