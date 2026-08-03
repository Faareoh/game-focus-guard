using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FocusTool.Ui;

internal sealed class UiCard : Panel
{
    private string _eyebrow = string.Empty;
    private string _title = string.Empty;

    public UiCard()
    {
        DoubleBuffered = true;
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);
        BackColor = UiTheme.Card;
        Padding = new Padding(16, 44, 16, 14);
        TabStop = false;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string Eyebrow
    {
        get => _eyebrow;
        set
        {
            _eyebrow = value;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using var borderPen = new Pen(UiTheme.Border);
        g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

        using var accentBrush = new SolidBrush(UiTheme.Accent);
        g.FillRectangle(accentBrush, 14, 15, 5, 18);

        using var eyebrowFont = UiTheme.Body(7f, FontStyle.Bold);
        using var titleFont = UiTheme.Display(13f, FontStyle.Bold);
        TextRenderer.DrawText(
            g,
            Eyebrow,
            eyebrowFont,
            new Point(27, 14),
            UiTheme.Muted,
            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding);
        TextRenderer.DrawText(
            g,
            Title,
            titleFont,
            new Point(27, 30),
            UiTheme.Ink,
            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding);
    }
}
