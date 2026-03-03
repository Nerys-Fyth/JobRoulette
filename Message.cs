using System;
using System.Windows.Forms;

namespace JobRoulette
{
    public partial class mbBody : Form
    {
        public mbBody() { InitializeComponent(); }

        private void mbOK_Click(object sender, EventArgs e) { this.DialogResult = DialogResult.OK; this.Close(); }

        private void mbCancel_Click(object sender, EventArgs e) { this.DialogResult = DialogResult.Cancel; this.Close(); }
    }
}
