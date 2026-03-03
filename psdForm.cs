using System;
using System.Windows.Forms;

namespace JobRoulette
{
    public partial class psdForm : Form
    {
        public psdForm() { InitializeComponent(); }

        private void psdInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                psdSaveButton_Click(this, null);
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void psdSaveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
