namespace floating_buttons;

public class KeystrokeButton : DraggableButton
{
    private readonly string _label;
    private readonly string _keystroke;

    public KeystrokeButton(string label, string keystroke)
    {
        _label = label;
        _keystroke = keystroke;

        SetVisuals();
    }

    private void SetVisuals()
    {
        this.Text = _label;
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);

        if (IsDragging) return;

        SendKeys.SendWait(_keystroke);
    }

    protected override void OnDrag(DragEventArgs e)
    {
        base.OnDrag(e);

        this.Left += e.DeltaX;
        this.Top += e.DeltaY;
    }
}