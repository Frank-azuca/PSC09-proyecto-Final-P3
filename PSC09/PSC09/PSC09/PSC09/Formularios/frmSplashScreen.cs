using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmSplashScreen : Form
    {
        public frmSplashScreen()
        {
            InitializeComponent();
        }

        private void tmrCarga_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(2);
            lblCarga.Text = progressBar1.Value.ToString() + "%";

            if (progressBar1.Value == progressBar1.Maximum)
            {
                tmrCarga.Stop();
                this.Hide();
                frmLogin login = new frmLogin();
                login.Show();
            }
        }
    }
}
