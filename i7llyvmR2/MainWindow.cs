namespace i7llyvmR2
{
    public partial class MainWindow : Form
    {
        protected override void WndProc(ref Message m)
        {
            const int HTCAPTION = 0x2;
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 0x1;
            const int WM_NCRBUTTONUP = 0x00A5;
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
    }
}
