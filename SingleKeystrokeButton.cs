using System.Drawing.Drawing2D;

namespace floating_buttons;

public class SingleKeystrokeButton : KeystrokeButton
{
    public SingleKeystrokeButton(string label, string keystroke) : base(label, keystroke)
    {
        SetVisuals();
    }

    private void SetVisuals()
    {
        this.Size = new Size(100, 100);
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;
        this.BackColor = Color.FromArgb(255, 100, 100, 100);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var bounds = new Rectangle(0, 0, this.Width, this.Height);
        var cornerRadiusX = bounds.Width;
        var cornerRadiusY = bounds.Height;

        var path = new GraphicsPath();

        path.StartFigure();

        AddCorner(0, 0, 180, 90);
        AddCorner(bounds.Width - cornerRadiusX, 0, 270, 90);
        AddCorner(bounds.Width - cornerRadiusX, bounds.Height - cornerRadiusY, 0, 90);
        AddCorner(0, bounds.Height - cornerRadiusY, 90, 90);

        path.CloseAllFigures();

        this.Region = new Region(path);

        base.OnPaint(pevent);

        var g = pevent.Graphics;

        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Draw border
        g.DrawPath(new Pen(Color.Black, 4), path);

        return;

        void AddCorner(int x, int y, float startAngle, float endAngle)
        {
            path.AddArc(
                bounds.X + x, bounds.Y + y,
                cornerRadiusX, cornerRadiusY,
                startAngle, endAngle);
        }
    }
}