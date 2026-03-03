namespace JobRoulette
{
    partial class mbBody
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
            this.mbOK = new System.Windows.Forms.Button();
            this.mbCancel = new System.Windows.Forms.Button();
            this.mbText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mbOK
            // 
            this.mbOK.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbOK.Location = new System.Drawing.Point(10, 401);
            this.mbOK.Margin = new System.Windows.Forms.Padding(1, 5, 1, 1);
            this.mbOK.Name = "mbOK";
            this.mbOK.Size = new System.Drawing.Size(75, 23);
            this.mbOK.TabIndex = 2;
            this.mbOK.Text = "OK";
            this.mbOK.UseVisualStyleBackColor = true;
            this.mbOK.Click += new System.EventHandler(this.mbOK_Click);
            // 
            // mbCancel
            // 
            this.mbCancel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbCancel.Location = new System.Drawing.Point(227, 401);
            this.mbCancel.Margin = new System.Windows.Forms.Padding(1, 5, 1, 1);
            this.mbCancel.Name = "mbCancel";
            this.mbCancel.Size = new System.Drawing.Size(75, 23);
            this.mbCancel.TabIndex = 3;
            this.mbCancel.Text = "Cancel";
            this.mbCancel.UseVisualStyleBackColor = true;
            this.mbCancel.Click += new System.EventHandler(this.mbCancel_Click);
            // 
            // mbText
            // 
            this.mbText.Font = new System.Drawing.Font("Segoe Print", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbText.Location = new System.Drawing.Point(12, 9);
            this.mbText.Name = "mbText";
            this.mbText.Size = new System.Drawing.Size(292, 387);
            this.mbText.TabIndex = 1;
            this.mbText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mbBody
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 434);
            this.ControlBox = false;
            this.Controls.Add(this.mbText);
            this.Controls.Add(this.mbCancel);
            this.Controls.Add(this.mbOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "mbBody";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Message Box";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbOK;
        private System.Windows.Forms.Button mbCancel;
        private System.Windows.Forms.Label mbText;
    }
}