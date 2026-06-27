using System;
using System.Drawing;
using System.Windows.Forms;

namespace Project_Utily
{
    public enum TrackBarOrientation
    {
        Horizontal,
        Vertical
    }

    public class CustomTrackBar : UserControl
    {
        private int minValue = 20;
        private int maxValue = 100;
        private int currentValue = 100;
        private bool isDragging = false;
        private TrackBarOrientation orientation = TrackBarOrientation.Horizontal;

        public new event EventHandler Scroll;

        public int Minimum { get => minValue; set { minValue = value; this.Invalidate(); } }
        public int Maximum { get => maxValue; set { maxValue = value; this.Invalidate(); } }
        public int Value
        {
            get => currentValue;
            set { currentValue = Math.Max(minValue, Math.Min(maxValue, value)); this.Invalidate(); }
        }

        public TrackBarOrientation Orientation
        {
            get => orientation;
            set { orientation = value; this.Invalidate(); }
        }

        public CustomTrackBar()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.Size = new Size(185, 20);
            this.BackColor = Color.FromArgb(25, 25, 25);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (orientation == TrackBarOrientation.Horizontal)
            {
                int trackY = this.Height / 2 - 2;
                using (Brush trackBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
                {
                    g.FillRectangle(trackBrush, 10, trackY, this.Width - 20, 4);
                }

                float percent = (float)(currentValue - minValue) / (maxValue - minValue);
                int thumbX = 10 + (int)(percent * (this.Width - 20));

                using (Brush fillBrush = new SolidBrush(Color.FromArgb(0, 122, 204)))
                {
                    g.FillRectangle(fillBrush, 10, trackY, thumbX - 10, 4);
                }

                using (Brush thumbBrush = new SolidBrush(Color.FromArgb(0, 150, 255)))
                {
                    g.FillEllipse(thumbBrush, thumbX - 6, this.Height / 2 - 6, 12, 12);
                }
            }
            else
            {
                using (Brush trackBrush = new SolidBrush(Color.FromArgb(35, 35, 35)))
                {
                    g.FillRectangle(trackBrush, 0, 0, this.Width, this.Height);
                }

                float percent = (float)(currentValue - minValue) / (maxValue - minValue);
                int thumbHeight = Math.Max(30, this.Height / 4);
                int thumbY = (int)(percent * (this.Height - thumbHeight));

                using (Brush thumbBrush = new SolidBrush(Color.FromArgb(65, 65, 65)))
                {
                    g.FillRectangle(thumbBrush, 1, thumbY, this.Width - 2, thumbHeight);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isDragging = true;
            UpdateValueFromMouse(e.X, e.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging) UpdateValueFromMouse(e.X, e.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isDragging = false;
        }

        private void UpdateValueFromMouse(int mouseX, int mouseY)
        {
            float percent;

            if (orientation == TrackBarOrientation.Horizontal)
            {
                percent = (float)(mouseX - 10) / (this.Width - 20);
            }
            else
            {
                int thumbHeight = Math.Max(30, this.Height / 4);
                percent = (float)(mouseY - thumbHeight / 2) / (this.Height - thumbHeight);
            }

            percent = Math.Max(0, Math.Min(1, percent));
            currentValue = minValue + (int)(percent * (maxValue - minValue));
            this.Invalidate();
            Scroll?.Invoke(this, EventArgs.Empty);
        }
    }
}