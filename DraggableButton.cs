namespace floating_buttons;

public class DragEventArgs : EventArgs
{
    public Point LastLocation { get; private set; }
    public Point CurrentLocation { get; private set; }
    public int DeltaX { get; private set; }
    public int DeltaY { get; private set; }

    public DragEventArgs(Point lastLocation, Point currentLocation)
    {
        LastLocation = lastLocation;
        CurrentLocation = currentLocation;
        DeltaX = currentLocation.X - lastLocation.X;
        DeltaY = currentLocation.Y - lastLocation.Y;
    }
}

public class DraggableButton : Button
{
    public bool IsDragging { get; private set; } = false;

    private bool _isMouseDown = false;
    private Point _lastLocation;
    private TimeSpan _clickTime;

    public event EventHandler DragStart = delegate { };
    public event EventHandler DragEnd = delegate { };
    public event EventHandler<DragEventArgs> Drag = delegate { };

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        _isMouseDown = true;
        _lastLocation = e.Location;
        _clickTime = DateTime.Now.TimeOfDay;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (IsDragging) OnDragEnd(EventArgs.Empty);

        _isMouseDown = false;
        IsDragging = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!_isMouseDown) return;

        if (!IsDragging)
        {
            IsDragging = DragCheck(e);

            if (IsDragging) OnDragStart(EventArgs.Empty);
            else return;
        }

        OnDrag(new DragEventArgs(_lastLocation, e.Location));
    }

    private bool DragCheck(MouseEventArgs e)
    {
        // Check if the mouse has moved more than 10 pixels
        var dx = e.Location.X - _lastLocation.X;
        var dy = e.Location.Y - _lastLocation.Y;

        // Check if the mouse has been held for more than 100ms
        var timediff = DateTime.Now.TimeOfDay - _clickTime;

        return dx + dy > 10 || timediff.TotalMilliseconds > 100;
    }

    protected virtual void OnDragStart(EventArgs e) => DragStart(this, e);
    protected virtual void OnDragEnd(EventArgs e) => DragEnd(this, e);
    protected virtual void OnDrag(DragEventArgs e) => Drag(this, e);
}