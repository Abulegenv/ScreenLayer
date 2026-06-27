using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace Project_Utily
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private WebView2 webView;
        private Panel configPanel;
        private TextBox wInput, hInput;
        private CustomTrackBar customSlider;
        private CustomTrackBar vScrollBar;
        private Button applyButton;
        private Button settingsButton;
        private Button btnHotkey1;
        private Button btnHotkey2;
        private Button btnPreset1;
        private Button btnPreset2;
        private Button btnPreset3;
        private NotifyIcon appTray;
        private ContextMenuStrip trayContext;
        private Label lblSize, lblW, lblH, lblOpacity, lblHK1, lblHK2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(480, 270);

            this.trayContext = new ContextMenuStrip();
            this.trayContext.Items.Add("Выйти", null, new System.EventHandler(this.ExitApp));

            this.appTray = new NotifyIcon(this.components);
            this.appTray.Text = "Project Utily";
            this.appTray.Icon = SystemIcons.Application;
            this.appTray.ContextMenuStrip = this.trayContext;
            this.appTray.Visible = true;
            this.appTray.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayClick);

            this.webView = new WebView2();
            this.webView.Location = new Point(0, 0);
            this.webView.Size = new Size(480, 270);
            this.Controls.Add(this.webView);

            this.settingsButton = new Button();
            this.settingsButton.Text = "⚙";
            this.settingsButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.settingsButton.ForeColor = Color.FromArgb(200, 200, 200);
            this.settingsButton.BackColor = Color.FromArgb(100, 20, 20, 20);
            this.settingsButton.FlatStyle = FlatStyle.Flat;
            this.settingsButton.FlatAppearance.BorderSize = 0;
            this.settingsButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(180, 40, 40, 40);
            this.settingsButton.Size = new Size(30, 30);
            this.settingsButton.Location = new Point(5, 5);
            this.settingsButton.Click += new System.EventHandler(this.ToggleSettings);
            this.Controls.Add(this.settingsButton);
            this.settingsButton.BringToFront();

            this.configPanel = new Panel();
            this.configPanel.Size = new Size(245, 270);
            this.configPanel.BackColor = Color.FromArgb(30, 30, 33);
            this.configPanel.Visible = false;
            this.configPanel.AutoScroll = false;

            this.vScrollBar = new CustomTrackBar();
            this.vScrollBar.Orientation = TrackBarOrientation.Vertical;
            this.vScrollBar.Size = new Size(8, 250);
            this.vScrollBar.Minimum = 0;
            this.vScrollBar.Maximum = 100;
            this.vScrollBar.Value = 0;
            this.vScrollBar.Visible = false;

            this.lblSize = new Label { Text = "WINDOW GEOMETRY", Location = new Point(16, 18), ForeColor = Color.FromArgb(114, 137, 218), Font = new Font("Segoe UI", 8F, FontStyle.Bold), Size = new Size(150, 16) };

            this.lblW = new Label { Text = "W:", Location = new Point(16, 44), ForeColor = Color.FromArgb(180, 180, 185), Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Size = new Size(22, 16) };
            this.wInput = new TextBox { Location = new Point(40, 41), Size = new Size(60, 23), Font = new Font("Segoe UI", 9F), BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center };
            this.wInput.KeyPress += new KeyPressEventHandler(this.SizeInput_KeyPress);

            this.lblH = new Label { Text = "H:", Location = new Point(116, 44), ForeColor = Color.FromArgb(180, 180, 185), Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Size = new Size(22, 16) };
            this.hInput = new TextBox { Location = new Point(140, 41), Size = new Size(60, 23), Font = new Font("Segoe UI", 9F), BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center };
            this.hInput.KeyPress += new KeyPressEventHandler(this.SizeInput_KeyPress);

            this.btnPreset1 = new Button { Text = "640x360", Location = new Point(16, 74), Size = new Size(62, 24), Font = new Font("Segoe UI", 8F), BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.FromArgb(220, 220, 225), FlatStyle = FlatStyle.Flat };
            this.btnPreset1.FlatAppearance.BorderSize = 0;
            this.btnPreset1.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            this.btnPreset1.Click += new System.EventHandler(this.BtnPreset_Click);

            this.btnPreset2 = new Button { Text = "480x270", Location = new Point(83, 74), Size = new Size(62, 24), Font = new Font("Segoe UI", 8F), BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.FromArgb(220, 220, 225), FlatStyle = FlatStyle.Flat };
            this.btnPreset2.FlatAppearance.BorderSize = 0;
            this.btnPreset2.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            this.btnPreset2.Click += new System.EventHandler(this.BtnPreset_Click);

            this.btnPreset3 = new Button { Text = "320x180", Location = new Point(150, 74), Size = new Size(62, 24), Font = new Font("Segoe UI", 8F), BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.FromArgb(220, 220, 225), FlatStyle = FlatStyle.Flat };
            this.btnPreset3.FlatAppearance.BorderSize = 0;
            this.btnPreset3.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            this.btnPreset3.Click += new System.EventHandler(this.BtnPreset_Click);

            this.lblOpacity = new Label { Text = "INTERFACE OPACITY", Location = new Point(16, 114), ForeColor = Color.FromArgb(114, 137, 218), Font = new Font("Segoe UI", 8F, FontStyle.Bold), Size = new Size(150, 16) };
            this.customSlider = new CustomTrackBar { Location = new Point(16, 134), Size = new Size(196, 20), Minimum = 20, Maximum = 100, Value = 100 };
            this.customSlider.Scroll += new System.EventHandler(this.CustomSlider_Scroll);

            this.lblHK1 = new Label { Text = "GLOBAL HOTKEY (SHOW / HIDE)", Location = new Point(16, 172), ForeColor = Color.FromArgb(114, 137, 218), Font = new Font("Segoe UI", 8F, FontStyle.Bold), Size = new Size(200, 16) };
            this.btnHotkey1 = new Button { Location = new Point(16, 192), Size = new Size(196, 28), Font = new Font("Segoe UI", 8.5F), BackColor = Color.FromArgb(40, 40, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            this.btnHotkey1.FlatAppearance.BorderSize = 0;
            this.btnHotkey1.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 55, 60);
            this.btnHotkey1.Click += new System.EventHandler(this.BtnHotkey1_Click);

            this.lblHK2 = new Label { Text = "CLICK-THROUGH (OVERLAY MODE)", Location = new Point(16, 232), ForeColor = Color.FromArgb(114, 137, 218), Font = new Font("Segoe UI", 8F, FontStyle.Bold), Size = new Size(200, 16) };
            this.btnHotkey2 = new Button { Location = new Point(16, 252), Size = new Size(196, 28), Font = new Font("Segoe UI", 8.5F), BackColor = Color.FromArgb(40, 40, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            this.btnHotkey2.FlatAppearance.BorderSize = 0;
            this.btnHotkey2.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 55, 60);
            this.btnHotkey2.Click += new System.EventHandler(this.BtnHotkey2_Click);

            this.applyButton = new Button { Text = "APPLY CHANGES", Location = new Point(16, 296), Size = new Size(196, 32), Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(88, 101, 242), FlatStyle = FlatStyle.Flat };
            this.applyButton.FlatAppearance.BorderSize = 0;
            this.applyButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 82, 196);
            this.applyButton.Click += new System.EventHandler(this.ApplySettings);

            this.Controls.Add(this.configPanel);
            this.configPanel.BringToFront();

            this.Load += new System.EventHandler(this.Form1_Load);
        }
    }
}