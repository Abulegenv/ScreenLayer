using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace Project_Utily
{
    public partial class Form1 : Form
    {
        private readonly string config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
        private bool isHidden = false;
        private bool clickThrough = false;
        private Keys hotkeyHideShow = Keys.F12;
        private Keys hotkeyClickThrough = Keys.F11;
        private int currentVideoWidth = 480;
        private int currentVideoHeight = 270;
        private int targetOpacity = 100;

        private bool isListeningHK1 = false;
        private bool isListeningHK2 = false;

        private Timer animationTimer;
        private Timer fadeInTimer;
        private bool isOpening = false;
        private int targetPanelX = 0;

        private Panel scrollContainer;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int index, int newLong);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public Form1()
        {
            this.Opacity = 0;
            InitializeComponent();
            SetupCustomScrollLayout();

            webView.CoreWebView2InitializationCompleted += WebView_InitializationCompleted;

            animationTimer = new Timer { Interval = 10 };
            animationTimer.Tick += AnimationTimer_Tick;

            fadeInTimer = new Timer { Interval = 15 };
            fadeInTimer.Tick += FadeInTimer_Tick;
        }

        private void SetupCustomScrollLayout()
        {
            scrollContainer = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(225, 340),
                BackColor = Color.Transparent
            };

            scrollContainer.Controls.Add(lblSize);
            scrollContainer.Controls.Add(lblW);
            scrollContainer.Controls.Add(wInput);
            scrollContainer.Controls.Add(lblH);
            scrollContainer.Controls.Add(hInput);
            scrollContainer.Controls.Add(btnPreset1);
            scrollContainer.Controls.Add(btnPreset2);
            scrollContainer.Controls.Add(btnPreset3);
            scrollContainer.Controls.Add(lblOpacity);
            scrollContainer.Controls.Add(customSlider);
            scrollContainer.Controls.Add(lblHK1);
            scrollContainer.Controls.Add(btnHotkey1);
            scrollContainer.Controls.Add(lblHK2);
            scrollContainer.Controls.Add(btnHotkey2);
            scrollContainer.Controls.Add(applyButton);

            configPanel.Controls.Add(scrollContainer);
            configPanel.Controls.Add(vScrollBar);

            configPanel.MouseDown += MoveWindow;
            scrollContainer.MouseDown += MoveWindow;
            lblSize.MouseDown += MoveWindow;
            lblW.MouseDown += MoveWindow;
            lblH.MouseDown += MoveWindow;
            lblOpacity.MouseDown += MoveWindow;
            lblHK1.MouseDown += MoveWindow;
            lblHK2.MouseDown += MoveWindow;

            vScrollBar.Scroll += VScrollBar_Scroll;

            configPanel.MouseWheel += Panel_MouseWheel;
            scrollContainer.MouseWheel += Panel_MouseWheel;
        }

        private void Panel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!vScrollBar.Visible) return;

            int change = e.Delta > 0 ? -10 : 10;
            int newValue = vScrollBar.Value + change;

            if (newValue < vScrollBar.Minimum) newValue = vScrollBar.Minimum;
            if (newValue > vScrollBar.Maximum) newValue = vScrollBar.Maximum;

            vScrollBar.Value = newValue;
            VScrollBar_Scroll(vScrollBar, EventArgs.Empty);
        }

        private void VScrollBar_Scroll(object sender, EventArgs e)
        {
            int maxScroll = scrollContainer.Height - configPanel.Height;
            if (maxScroll > 0)
            {
                float percent = (float)(vScrollBar.Value - vScrollBar.Minimum) / (vScrollBar.Maximum - vScrollBar.Minimum);
                scrollContainer.Location = new Point(0, -(int)(percent * maxScroll));
            }
        }

        private void FadeInTimer_Tick(object sender, EventArgs e)
        {
            double step = 0.05;
            double target = targetOpacity / 100.0;
            if (this.Opacity < target)
            {
                this.Opacity = Math.Min(this.Opacity + step, target);
            }
            else
            {
                fadeInTimer.Stop();
            }
        }

        private void WebView_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView.Size = this.Size;
                webView.Update();
            }
        }

        private void ToggleSettings(object sender, EventArgs e)
        {
            if (animationTimer.Enabled) return;

            if (!configPanel.Visible)
            {
                Rectangle screen = Screen.FromControl(this).Bounds;
                currentVideoWidth = this.Width;
                currentVideoHeight = this.Height;
                isOpening = true;

                wInput.Text = currentVideoWidth.ToString();
                hInput.Text = currentVideoHeight.ToString();

                configPanel.Height = this.Height;

                if (scrollContainer.Height > configPanel.Height)
                {
                    vScrollBar.Visible = true;
                    vScrollBar.Height = configPanel.Height - 10;
                    vScrollBar.Location = new Point(configPanel.Width - vScrollBar.Width - 4, 5);
                    vScrollBar.Minimum = 0;
                    vScrollBar.Maximum = 100;
                    vScrollBar.Value = 0;
                }
                else
                {
                    vScrollBar.Visible = false;
                }
                scrollContainer.Location = new Point(0, 0);

                if (screen.Right - this.Bounds.Right < configPanel.Width + 10)
                {
                    this.Location = new Point(this.Location.X - configPanel.Width, this.Location.Y);
                    this.Size = new Size(currentVideoWidth + configPanel.Width, this.Height);
                    webView.Location = new Point(configPanel.Width, 0);
                    configPanel.Location = new Point(-configPanel.Width, 0);
                    targetPanelX = 0;
                }
                else
                {
                    this.Size = new Size(currentVideoWidth + configPanel.Width, this.Height);
                    webView.Location = new Point(0, 0);
                    configPanel.Location = new Point(this.Width, 0);
                    targetPanelX = currentVideoWidth;
                }

                configPanel.Visible = true;
                configPanel.BringToFront();
                settingsButton.BringToFront();
                animationTimer.Start();
            }
            else
            {
                isOpening = false;
                animationTimer.Start();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            int speed = 30;
            if (isOpening)
            {
                if (configPanel.Location.X < targetPanelX)
                {
                    configPanel.Location = new Point(Math.Min(configPanel.Location.X + speed, targetPanelX), 0);
                }
                else if (configPanel.Location.X > targetPanelX)
                {
                    configPanel.Location = new Point(Math.Max(configPanel.Location.X - speed, targetPanelX), 0);
                }
                else
                {
                    animationTimer.Stop();
                    settingsButton.Location = (configPanel.Location.X == 0) ? new Point(this.Width - 35, 5) : new Point(5, 5);
                }
            }
            else
            {
                int closeTarget = (configPanel.Location.X <= 0) ? -configPanel.Width : this.Width;
                if (configPanel.Location.X > closeTarget && closeTarget == -configPanel.Width)
                {
                    configPanel.Location = new Point(configPanel.Location.X - speed, 0);
                }
                else if (configPanel.Location.X < closeTarget && closeTarget == this.Width)
                {
                    configPanel.Location = new Point(configPanel.Location.X + speed, 0);
                }
                else
                {
                    animationTimer.Stop();
                    CloseSettingsWithoutSave();
                }
            }
        }

        private void MoveWindow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xA1, 0x2, 0);
                Save();
            }
        }

        private void CustomSlider_Scroll(object sender, EventArgs e)
        {
            targetOpacity = customSlider.Value;
            this.Opacity = targetOpacity / 100.0;
        }

        private void SizeInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void BtnPreset_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                string[] sizes = btn.Text.Split('x');
                if (sizes.Length == 2)
                {
                    wInput.Text = sizes[0];
                    hInput.Text = sizes[1];
                }
            }
        }

        private void BtnHotkey1_Click(object sender, EventArgs e)
        {
            isListeningHK1 = true;
            isListeningHK2 = false;
            btnHotkey1.Text = "[ Нажмите клавишу ]";
            btnHotkey1.BackColor = Color.FromArgb(0, 122, 204);
            btnHotkey2.Text = $"Ctrl + {hotkeyClickThrough}";
            btnHotkey2.BackColor = Color.FromArgb(40, 40, 43);
        }

        private void BtnHotkey2_Click(object sender, EventArgs e)
        {
            isListeningHK2 = true;
            isListeningHK1 = false;
            btnHotkey2.Text = "[ Нажмите клавишу ]";
            btnHotkey2.BackColor = Color.FromArgb(0, 122, 204);
            btnHotkey1.Text = $"Ctrl + {hotkeyHideShow}";
            btnHotkey1.BackColor = Color.FromArgb(40, 40, 43);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys baseKey = keyData & Keys.KeyCode;

            if (isListeningHK1 && baseKey != Keys.ControlKey && baseKey != Keys.ShiftKey && baseKey != Keys.Menu)
            {
                hotkeyHideShow = baseKey;
                btnHotkey1.Text = $"Ctrl + {hotkeyHideShow}";
                btnHotkey1.BackColor = Color.FromArgb(40, 40, 43);
                isListeningHK1 = false;
                return true;
            }
            if (isListeningHK2 && baseKey != Keys.ControlKey && baseKey != Keys.ShiftKey && baseKey != Keys.Menu)
            {
                hotkeyClickThrough = baseKey;
                btnHotkey2.Text = $"Ctrl + Alt + {hotkeyClickThrough}";
                btnHotkey2.BackColor = Color.FromArgb(40, 40, 43);
                isListeningHK2 = false;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ApplySettings(object sender, EventArgs e)
        {
            if (int.TryParse(wInput.Text, out int w) && int.TryParse(hInput.Text, out int h))
            {
                Rectangle maxBounds = Screen.FromControl(this).Bounds;

                w = Math.Max(160, Math.Min(maxBounds.Width, w));
                h = Math.Max(90, Math.Min(maxBounds.Height, h));

                configPanel.Visible = false;
                currentVideoWidth = w;
                currentVideoHeight = h;
                this.Size = new Size(w, h);
                webView.Location = new Point(0, 0);
                webView.Size = this.Size;
                settingsButton.Location = new Point(5, 5);

                ResetHotkeys();
                SetWindowClickThrough(clickThrough);
                Save();
            }
        }

        private void CloseSettingsWithoutSave()
        {
            configPanel.Visible = false;
            if (configPanel.Location.X <= 0)
            {
                this.Location = new Point(this.Location.X + configPanel.Width, this.Location.Y);
            }
            this.Size = new Size(currentVideoWidth, currentVideoHeight);
            webView.Location = new Point(0, 0);
            webView.Size = this.Size;
            settingsButton.Location = new Point(5, 5);

            isListeningHK1 = false;
            isListeningHK2 = false;
            btnHotkey1.BackColor = Color.FromArgb(40, 40, 43);
            btnHotkey2.BackColor = Color.FromArgb(40, 40, 43);
        }

        private void SetWindowClickThrough(bool enable)
        {
            int style = GetWindowLong(this.Handle, -20);
            if (enable)
            {
                SetWindowLong(this.Handle, -20, style | 0x00080000 | 0x00000020);
            }
            else
            {
                SetWindowLong(this.Handle, -20, style & ~0x00000020);
            }
        }

        private void ResetHotkeys()
        {
            UnregisterHotKey(this.Handle, 1);
            UnregisterHotKey(this.Handle, 2);
            RegisterHotKey(this.Handle, 1, 0x0002, (int)hotkeyHideShow);
            RegisterHotKey(this.Handle, 2, 0x0002 | 0x0001, (int)hotkeyClickThrough);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();
                if (id == 1)
                {
                    if (isHidden)
                    {
                        SetWindowClickThrough(false);
                        this.Show();
                        this.WindowState = FormWindowState.Normal;
                        if (webView != null) webView.Visible = true;
                        fadeInTimer.Start();
                    }
                    else
                    {
                        if (clickThrough) SetWindowClickThrough(true);
                        if (webView != null) webView.Visible = false;
                        this.Hide();
                    }
                    isHidden = !isHidden;
                    return;
                }
                else if (id == 2)
                {
                    clickThrough = !clickThrough;
                    SetWindowClickThrough(clickThrough);
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;

            LoadConfig();
            ResetHotkeys();
            SetWindowClickThrough(clickThrough);

            fadeInTimer.Start();

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string userDataFolder = Path.Combine(localAppData, "ScreenLayer", "WebView2Profile");
                if (!Directory.Exists(userDataFolder)) Directory.CreateDirectory(userDataFolder);

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);

                webView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0";
                webView.CoreWebView2.Navigate("https://www.youtube.com");
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TrayClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleSettings(this, EventArgs.Empty);
            }
        }

        private void ExitApp(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Save()
        {
            try
            {
                string[] data = {
                    $"{this.Location.X}", $"{this.Location.Y}", $"{currentVideoWidth}", $"{currentVideoHeight}",
                    $"{targetOpacity}", $"{(int)hotkeyHideShow}", clickThrough ? "1" : "0", $"{(int)hotkeyClickThrough}"
                };
                File.WriteAllLines(config, data);
            }
            catch { }
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(config))
                {
                    string[] lines = File.ReadAllLines(config);
                    if (lines.Length >= 8)
                    {
                        Point savedLocation = new Point(int.Parse(lines[0]), int.Parse(lines[1]));

                        bool isPointVisible = false;
                        foreach (var screen in Screen.AllScreens)
                        {
                            if (screen.Bounds.Contains(savedLocation))
                            {
                                isPointVisible = true;
                                break;
                            }
                        }

                        this.Location = isPointVisible ? savedLocation : new Point(50, 50);
                        currentVideoWidth = int.Parse(lines[2]);
                        currentVideoHeight = int.Parse(lines[3]);
                        this.Size = new Size(currentVideoWidth, currentVideoHeight);
                        targetOpacity = int.Parse(lines[4]);
                        customSlider.Value = targetOpacity;
                        hotkeyHideShow = (Keys)int.Parse(lines[5]);
                        btnHotkey1.Text = $"Ctrl + {hotkeyHideShow}";
                        clickThrough = lines[6] == "1";
                        hotkeyClickThrough = (Keys)int.Parse(lines[7]);
                        btnHotkey2.Text = $"Ctrl + Alt + {hotkeyClickThrough}";
                        return;
                    }
                }
            }
            catch { }

            this.Location = new Point(50, 50);
            currentVideoWidth = 480;
            currentVideoHeight = 270;
            this.Size = new Size(currentVideoWidth, currentVideoHeight);
            targetOpacity = 100;
            customSlider.Value = 100;
            hotkeyHideShow = Keys.F12;
            btnHotkey1.Text = "Ctrl + F12";
            clickThrough = false;
            hotkeyClickThrough = Keys.F11;
            btnHotkey2.Text = "Ctrl + Alt + F11";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 1);
            UnregisterHotKey(this.Handle, 2);
            appTray.Dispose();
            base.OnFormClosing(e);
        }
    }
}