
using static System.Net.Mime.MediaTypeNames;

namespace i7llyvmR2
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            notifyIcon = new NotifyIcon(components);
            lineLabel = new Label();
            descrLaerl = new Label();
            AppsPlusLable = new Label();
            buttonsStatisticsLabel = new Label();
            errorLabel = new Label();
            triggerStatisticsLabel = new Label();
            updateTimeLabel = new Label();
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "i7llymTray";
            // 
            // lineLabel
            // 
            lineLabel.BorderStyle = BorderStyle.Fixed3D;
            lineLabel.Location = new Point(-1, 322);
            lineLabel.Name = "lineLabel";
            lineLabel.Size = new Size(700, 2);
            lineLabel.TabIndex = 0;
            // 
            // descrLaerl
            // 
            descrLaerl.AutoSize = true;
            descrLaerl.ForeColor = SystemColors.HotTrack;
            descrLaerl.Location = new Point(325, 324);
            descrLaerl.Name = "descrLaerl";
            descrLaerl.Size = new Size(374, 15);
            descrLaerl.TabIndex = 1;
            descrLaerl.Text = "\\ - Quit,  \" - Maximize,  / - Hide,  Right Mouse - Stay on top,  + - Clear";
            // 
            // AppsPlusLable
            // 
            AppsPlusLable.AutoSize = true;
            AppsPlusLable.BackColor = Color.Gainsboro;
            AppsPlusLable.ForeColor = SystemColors.HotTrack;
            AppsPlusLable.Location = new Point(-1, 323);
            AppsPlusLable.Name = "AppsPlusLable";
            AppsPlusLable.Size = new Size(48, 15);
            AppsPlusLable.TabIndex = 2;
            AppsPlusLable.Text = "Apps + ";
            // 
            // buttonsStatisticsLabel
            // 
            buttonsStatisticsLabel.AutoSize = true;
            buttonsStatisticsLabel.BackColor = SystemColors.ScrollBar;
            buttonsStatisticsLabel.BorderStyle = BorderStyle.FixedSingle;
            buttonsStatisticsLabel.ForeColor = SystemColors.HotTrack;
            buttonsStatisticsLabel.Location = new Point(10, 22);
            buttonsStatisticsLabel.Name = "buttonsStatisticsLabel";
            buttonsStatisticsLabel.Size = new Size(118, 47);
            buttonsStatisticsLabel.TabIndex = 3;
            buttonsStatisticsLabel.Text = "A: 0 B: 0 X: 0 Y: 0\r\nLB: 0 RB: 0 LS: 0 RS: 0\r\nStart: 0 Back: 0";
            // 
            // errorLabel
            // 
            errorLabel.AutoSize = true;
            errorLabel.Location = new Point(-1, 300);
            errorLabel.Name = "errorLabel";
            errorLabel.Size = new Size(51, 15);
            errorLabel.TabIndex = 4;
            errorLabel.Text = "No Error";
            // 
            // triggerStatisticsLabel
            // 
            triggerStatisticsLabel.AutoSize = true;
            triggerStatisticsLabel.BackColor = SystemColors.ScrollBar;
            triggerStatisticsLabel.BorderStyle = BorderStyle.FixedSingle;
            triggerStatisticsLabel.Location = new Point(10, 89);
            triggerStatisticsLabel.Name = "triggerStatisticsLabel";
            triggerStatisticsLabel.Size = new Size(59, 17);
            triggerStatisticsLabel.TabIndex = 7;
            triggerStatisticsLabel.Text = "RT: 0 LT: 0";
            // 
            // updateTimeLabel
            // 
            updateTimeLabel.AutoSize = true;
            updateTimeLabel.Location = new Point(602, 300);
            updateTimeLabel.Name = "updateTimeLabel";
            updateTimeLabel.Size = new Size(25, 15);
            updateTimeLabel.TabIndex = 8;
            updateTimeLabel.Text = "000";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gainsboro;
            ClientSize = new Size(700, 338);
            Controls.Add(updateTimeLabel);
            Controls.Add(triggerStatisticsLabel);
            Controls.Add(errorLabel);
            Controls.Add(buttonsStatisticsLabel);
            Controls.Add(AppsPlusLable);
            Controls.Add(descrLaerl);
            Controls.Add(lineLabel);
            Enabled = false;
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MaximumSize = new Size(700, 338);
            MinimumSize = new Size(700, 338);
            Name = "MainWindow";
            Text = "i7llyvmR2";
            Resize += MainWindow_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lineLabel;
        private Label descrLaerl;
        private Label buttonsStatisticsLabel;
        private Label errorLabel;
        private Label updateTimeLabel;
        private Label triggerStatisticsLabel;
        public delegate void Defaulti7Delegate();
        public event Defaulti7Delegate appManuallyExitEvent;
        public event Defaulti7Delegate clearStatisticsEvent;

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }
        private void SetTextInUIThread(Label l, string t) => l.BeginInvoke((MethodInvoker)delegate
        {
            l.Text = t;
        });

        public void ManuallyExit()
        {
            this.appManuallyExitEvent();
        }

        public void ClearStatistics()
        {
            this.clearStatisticsEvent();
        }

        public void SetButtonsLabel(string txt, bool currentThread = false)
        {
            if (currentThread)
            {
                this.buttonsStatisticsLabel.Text = txt;
            } else
            {
                SetTextInUIThread(this.buttonsStatisticsLabel, txt);
            }                
        }

        public void SetTriggersLabel(string txt, bool currentThread = false)
        {
            if (currentThread)
            {
                this.triggerStatisticsLabel.Text = txt;
            } else
            {
                SetTextInUIThread(this.triggerStatisticsLabel, txt);
            }                
        }

        public void SetErrorLabel(string txt, bool currentThread = false)
        {
            if (currentThread)
            {
                this.errorLabel.Text = txt;
            } else
            {
                SetTextInUIThread(this.errorLabel, txt);
            }                
        }        

        public void SetUpdateTimeLabel(string txt, bool currentThread = false)
        {
            if (currentThread)
            {
                this.updateTimeLabel.Text = txt;
            } else 
            {
                SetTextInUIThread(this.updateTimeLabel, txt);
            }
        }

        public NotifyIcon notifyIcon;
        public Label AppsPlusLable;
    }
}
