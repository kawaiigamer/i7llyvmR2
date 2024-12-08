using System.Runtime.InteropServices;

namespace i7llyvmR2
{
    public partial class MainWindow : Form
    {
        private const int HTCAPTION = 0x2;
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int WM_NCRBUTTONUP = 0x00A5;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {

                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if ((int)m.Result == HTCLIENT)
                        m.Result = (IntPtr)HTCAPTION;
                    return;

                case WM_NCRBUTTONUP:
                    this.TopMost = !this.TopMost;
                    break;
            }

            base.WndProc(ref m);
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void clearButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.clearStatisticsEvent();
        }

    }
}
