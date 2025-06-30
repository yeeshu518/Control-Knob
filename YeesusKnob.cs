using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace YeesusControls
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LabelItem
    {
        public LabelItem() { }
        public LabelItem(string text) => Text = text;

        [DisplayName("Text")]
        public string Text { get; set; } = "";

        public override string ToString() => Text;
    }

    [DesignTimeVisible(true)]
    [DefaultProperty("Labels")]
    public partial class YeesusKnob : UserControl
    {
        private const float StartAngle = 0f;
        private const float SweepAngle = 270f;
        private const int TickLength = 12;
        private const int BottomArea = 30;
        private const int TopArea = 20;

        private float _angle = StartAngle;
        private bool _dragging;
        private int _idx;

        private BindingList<LabelItem> _labels = new BindingList<LabelItem>();

        public YeesusKnob()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            Caption = "Sensitivity";
            PointerColor = Color.Red;
            ForeColor = Color.White;
            LabelFont = new Font("Segoe UI", 8f);
            ValueFont = new Font("Segoe UI", 12f, FontStyle.Bold);
            CaptionFont = new Font("Segoe UI", 10f);
            MinimumSize = new Size(100, 100);
            Size = new Size(180, 220);
            Value = 0;
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BindingList<LabelItem> Labels => _labels;

        [Browsable(false)]
        public string SelectedValue =>
            (_labels.Count > 0 && _idx < _labels.Count) ? _labels[_idx].Text : "";

        [Category("Appearance")]
        public string Caption { get; set; }

        [Category("Appearance")]
        public Font LabelFont { get; set; }

        [Category("Appearance")]
        public Font ValueFont { get; set; }

        [Category("Appearance")]
        public Font CaptionFont { get; set; }

        [Category("Appearance")]
        public Color PointerColor { get; set; }

        [Category("Behavior")]
        public event EventHandler ValueChanged;

        [Browsable(false)]
        public int Value
        {
            get => _idx;
            private set
            {
                _idx = Math.Max(0, Math.Min(_labels.Count - 1, value));
                _angle = StartAngle + _idx * (SweepAngle / Math.Max(1, _labels.Count - 1));
                Invalidate();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int W = Width;
            int H = Height;
            int dialH = H - BottomArea - TopArea;
            int R = Math.Min(W, dialH) / 2 - 10;
            int r = R - 20;
            var center = new Point(W / 2, TopArea + dialH / 2);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            float step = _labels.Count > 1 ? SweepAngle / (_labels.Count - 1) : 0;
            using var lb = new SolidBrush(ForeColor);

            for (int i = 0; i < _labels.Count; i++)
            {
                float a = StartAngle + i * step;
                var p1 = PointOnCircle(center, R, a);
                var p2 = PointOnCircle(center, R + TickLength, a);
                g.DrawLine(Pens.White, p1, p2);
                var lp = PointOnCircle(center, R + TickLength + 12, a);
                g.DrawString(_labels[i].Text, LabelFont, lb, lp, sf);
            }

            if (_labels.Count > 1)
            {
                using var pen = new Pen(Interpolate(Color.LightGray, Color.Red, _idx / (float)(_labels.Count - 1)), 6) { LineJoin = LineJoin.Round };
                g.DrawArc(pen, center.X - R, center.Y - R, R * 2, R * 2, 270, _angle - StartAngle);
            }

            var cr = new Rectangle(center.X - r, center.Y - r, r * 2, r * 2);
            using var capBrush = new LinearGradientBrush(cr, Color.LightGray, Color.DarkGray, 90f);
            g.FillEllipse(capBrush, cr);

            var dot = PointOnCircle(center, r - 8, _angle);
            using var pb = new SolidBrush(PointerColor);
            g.FillEllipse(pb, dot.X - 6, dot.Y - 6, 12, 12);

            g.DrawString(SelectedValue, ValueFont, lb, center, sf);

            var capArea = new RectangleF(0, TopArea + dialH + 7, W, BottomArea);
            g.DrawString(Caption, CaptionFont, lb, capArea, sf);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _dragging = true;
            Capture = true;
            Track(e.Location, false);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging) Track(e.Location, false);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false;
            Capture = false;
            Track(e.Location, true);
        }

        private void Track(Point m, bool snap)
        {
            if (_labels.Count < 2) return;

            var center = new Point(Width / 2, TopArea + (Height - BottomArea - TopArea) / 2);
            double dx = m.X - center.X;
            double dy = m.Y - center.Y;
            double deg = (Math.Atan2(dy, dx) * 180 / Math.PI + 90 + 360) % 360;
            deg = Math.Clamp(deg, StartAngle, StartAngle + SweepAngle);

            if (snap)
            {
                int idx = (int)Math.Round((deg - StartAngle) / (SweepAngle / (_labels.Count - 1)));
                if (idx != _idx) SystemSounds.Beep.Play();
                Value = idx;
            }
            else
            {
                _angle = (float)deg;
                Invalidate();
            }
        }

        private static Point PointOnCircle(Point c, float rad, float angDeg)
        {
            double radian = (angDeg - 90) * Math.PI / 180;
            return new Point(
                c.X + (int)(Math.Cos(radian) * rad),
                c.Y + (int)(Math.Sin(radian) * rad)
            );
        }

        private static Color Interpolate(Color a, Color b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t)
            );
        }
    }
}
