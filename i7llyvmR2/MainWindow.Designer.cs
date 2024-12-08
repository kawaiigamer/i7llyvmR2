
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
            label1 = new Label();
            descrLaerl = new Label();
            descrLabel2 = new Label();
            buttonsStatisticsLabel = new Label();
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.Text = "i7llymTray";
            // 
            // label1
            // 
            label1.BorderStyle = BorderStyle.Fixed3D;
            label1.Location = new Point(-1, 429);
            label1.Name = "label1";
            label1.Size = new Size(800, 2);
            label1.TabIndex = 0;
            // 
            // descrLaerl
            // 
            descrLaerl.AutoSize = true;
            descrLaerl.Location = new Point(400, 431);
            descrLaerl.Name = "descrLaerl";
            descrLaerl.Size = new Size(399, 20);
            descrLaerl.TabIndex = 1;
            descrLaerl.Text = "\\ - Quit,  \" - Maximize,  / - Hide,  Right Mouse - Stay on top";
            // 
            // descrLabel2
            // 
            descrLabel2.AutoSize = true;
            descrLabel2.Location = new Point(-1, 431);
            descrLabel2.Name = "descrLabel2";
            descrLabel2.Size = new Size(61, 20);
            descrLabel2.TabIndex = 2;
            descrLabel2.Text = "Apps + ";
            // 
            // buttonsStatisticsLabel
            // 
            buttonsStatisticsLabel.AutoSize = true;
            buttonsStatisticsLabel.Image = Properties.Resources._510xolNamXL;
            buttonsStatisticsLabel.Location = new Point(12, 30);
            buttonsStatisticsLabel.Name = "buttonsStatisticsLabel";
            buttonsStatisticsLabel.Size = new Size(147, 60);
            buttonsStatisticsLabel.TabIndex = 3;
            buttonsStatisticsLabel.Text = "A: 0 B: 0 X: 0 Y: 0\r\nLB: 0 RB: 0 LS: 0 RS: 0\r\nStart: 0 Back: 0";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonsStatisticsLabel);
            Controls.Add(descrLabel2);
            Controls.Add(descrLaerl);
            Controls.Add(label1);
            Enabled = false;
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(800, 450);
            MinimumSize = new Size(800, 450);
            Name = "MainWindow";
            Text = "i7llyvmR2";
            Resize += MainWindow_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NotifyIcon notifyIcon;
        private Label label1;
        private Label descrLaerl;
        private Label descrLabel2;
        public Label buttonsStatisticsLabel;
    }
}
