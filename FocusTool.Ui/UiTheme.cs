using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FocusTool.Ui;

internal static class UiTheme
{
    public static readonly Color Background = Color.FromArgb(237, 242, 248);
    public static readonly Color Card = Color.FromArgb(251, 252, 254);
    public static readonly Color CardAlt = Color.FromArgb(242, 246, 251);
    public static readonly Color Ink = Color.FromArgb(38, 51, 61);
    public static readonly Color Muted = Color.FromArgb(96, 120, 140);
    public static readonly Color Faint = Color.FromArgb(147, 165, 183);
    public static readonly Color Accent = Color.FromArgb(63, 122, 166);
    public static readonly Color AccentHover = Color.FromArgb(53, 107, 146);
    public static readonly Color Border = Color.FromArgb(212, 224, 235);
    public static readonly Color Success = Color.FromArgb(47, 125, 111);
    public static readonly Color SuccessHover = Color.FromArgb(38, 105, 93);
    public static readonly Color Danger = Color.FromArgb(179, 92, 92);
    public static readonly Color DangerHover = Color.FromArgb(155, 78, 78);
    public static readonly Color Terminal = Color.FromArgb(27, 40, 51);
    public static readonly Color TerminalText = Color.FromArgb(220, 232, 242);

    public static Font Display(float size, FontStyle style = FontStyle.Regular)
    {
        return new Font("Georgia", size, style, GraphicsUnit.Point);
    }

    public static Font Body(float size, FontStyle style = FontStyle.Regular)
    {
        return new Font("Microsoft YaHei UI", size, style, GraphicsUnit.Point);
    }

    public static Font Mono(float size, FontStyle style = FontStyle.Regular)
    {
        return new Font("Consolas", size, style, GraphicsUnit.Point);
    }

    public static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            path.CloseFigure();
            return path;
        }

        int diameter = radius * 2;
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void StylePrimaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = AccentHover;
        button.FlatAppearance.MouseDownBackColor = AccentHover;
        button.BackColor = Accent;
        button.ForeColor = Color.White;
        button.Font = Body(10.5f, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
        button.TabStop = true;
    }

    public static void StyleSecondaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Border;
        button.FlatAppearance.MouseOverBackColor = CardAlt;
        button.FlatAppearance.MouseDownBackColor = CardAlt;
        button.BackColor = Card;
        button.ForeColor = Ink;
        button.Font = Body(9.5f, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
        button.TabStop = true;
    }
}
