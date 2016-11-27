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
            ((System.ComponentModel.ISupportInitialize)(this.DGVCounterShow)).BeginInit();
            this.SuspendLayout();
            // 
            // DGVCounterShow
            // 
            this.DGVCounterShow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVCounterShow.Location = new System.Drawing.Point(0, 40);
            this.DGVCounterShow.Name = "DGVCounterShow";
            this.DGVCounterShow.RowTemplate.Height = 23;
            this.DGVCounterShow.Size = new System.Drawing.Size(744, 398);
            this.DGVCounterShow.TabIndex = 0;
            // 
            // BTNLoading
            // 
            this.BTNLoading.Location = new System.Drawing.Point(11, 7);
            this.BTNLoading.Name = "BTNLoading";
            this.BTNLoading.Size = new System.Drawing.Size(75, 23);
            this.BTNLoading.TabIndex = 1;
            this.BTNLoading.Text = "加载";
            this.BTNLoading.UseVisualStyleBackColor = true;
            this.BTNLoading.Click += new System.EventHandler(this.BTNLoading_Click);
            // 
            // UpdateCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 440);
            this.Controls.Add(this.BTNLoading);
            this.Controls.Add(this.DGVCounterShow);
            this.Name = "UpdateCheck";
            this.Text = "UpdateCheck";
            ((System.ComponentModel.ISupportInitialize)(this.DGVCounterShow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DGVCounterShow;
        private System.Windows.Forms.Button BTNLoading;
    }
}