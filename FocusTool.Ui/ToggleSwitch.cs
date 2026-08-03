using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FocusTool.Ui;

internal sealed class ToggleSwitch : Control
{
    private bool _checked;

    public ToggleSwitch()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);
        BackColor = UiTheme.Card;
        Size = new Size(46, 28);
        TabStop = true;
        Cursor = Cursors.Hand;
        AccessibleRole = AccessibleRole.CheckButton;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [DefaultValue(false)]
    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked == value)
            {
                return;
            }

            _checked = value;
            OnCheckedChanged(EventArgs.Empty);
            Invalidate();
        }
    }

    public event EventHandler? CheckedChanged;

    protected override void OnClick(EventArgs e)
    {
        Checked = !Checked;
        base.OnClick(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
        {
            Checked = !Checked;
            e.Handled = true;
            e.SuppressKeyPress = true;
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var track = new Rectangle(1, (Height - 22) / 2, 44, 22);
        using var trackPath = UiTheme.RoundedRectangle(track, 11);
        using var trackBrush = new SolidBrush(Checked ? UiTheme.Success : UiTheme.Border);
        g.FillPath(trackBrush, trackPath);
        using var trackPen = new Pen(Checked ? UiTheme.Success : UiTheme.Faint);
        g.DrawPath(trackPen, trackPath);

        const int knobSize = 16;
        var x = Checked ? track.Right - knobSize - 3 : track.Left + 3;
        var y = track.Top + (track.Height - knobSize) / 2;
        using var knobPath = UiTheme.RoundedRectangle(new Rectangle(x, y, knobSize, knobSize), 8);
        using var knobBrush = new SolidBrush(Color.White);
        g.FillPath(knobBrush, knobPath);

        if (Focused)
        {
            using var focusPen = new Pen(UiTheme.Accent, 1f)
            {
                DashStyle = DashStyle.Dot
            };
            g.DrawRectangle(focusPen, 1, 1, Width - 3, Height - 3);
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        Invalidate();
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        Invalidate();
        base.OnLostFocus(e);
    }

    private void OnCheckedChanged(EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }
}
