using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void psdSaveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
