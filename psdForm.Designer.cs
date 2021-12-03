
namespace JobRoulette
{
    partial class psdForm
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
            this.psdInput = new System.Windows.Forms.TextBox();
            this.psdSaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // psdInput
            // 
            this.psdInput.Location = new System.Drawing.Point(5, 5);
            this.psdInput.Margin = new System.Windows.Forms.Padding(5);
            this.psdInput.Name = "psdInput";
            this.psdInput.Size = new System.Drawing.Size(200, 20);
            this.psdInput.TabIndex = 0;
            this.psdInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.psdInput_KeyDown);
            // 
            // psdSaveButton
            // 
            this.psdSaveButton.Location = new System.Drawing.Point(209, 3);
            this.psdSaveButton.Name = "psdSaveButton";
            this.psdSaveButton.Size = new System.Drawing.Size(50, 23);
            this.psdSaveButton.TabIndex = 1;
            this.psdSaveButton.Text = "Save";
            this.psdSaveButton.UseVisualStyleBackColor = true;
            this.psdSaveButton.Click += new System.EventHandler(this.psdSaveButton_Click);
            // 
            // psdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(262, 29);
            this.ControlBox = false;
            this.Controls.Add(this.psdSaveButton);
            this.Controls.Add(this.psdInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "psdForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox psdInput;
        private System.Windows.Forms.Button psdSaveButton;
    }
}