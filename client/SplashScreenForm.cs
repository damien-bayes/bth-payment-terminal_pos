#region directives
using System;
using System.Reflection;
using System.Windows.Forms;
#endregion

namespace IPTS
{
    public partial class SplashScreenForm : Form
    {
        public SplashScreenForm()
        {
            InitializeComponent();

            version.Text = string.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        public void Kill(object sender, EventArgs e)
        {
            this.Close();
            this.Cursor = Cursors.Default;
        }

        public void BringTop(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.TopMost = true;
        }

        public void ChangeMessageText(object sender, EventArgs e)
        {
            message.Text = sender as String;
        }
    }
}
