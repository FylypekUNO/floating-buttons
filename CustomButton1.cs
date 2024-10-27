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

public class CustomButton1 : Button
{
    private bool isMouseDown = false;
    private bool isDragging = false;

    private Point lastLocation;
    private TimeSpan clickTime;

    public event EventHandler DragStart = delegate { };
    public event EventHandler DragEnd = delegate { };
    public event EventHandler<DragEventArgs> Drag = delegate { };
    public event EventHandler NonDragClick = delegate { };

    public CustomButton1(String text)
    {
        this.Text = text;

        SetVisuals();
    }

    private void SetVisuals()
    {
        this.FlatStyle = FlatStyle.Flat; // Remove button border
        this.BackColor = Color.LightBlue; // Initial color
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        isMouseDown = true;
        lastLocation = e.Location;
        clickTime = DateTime.Now.TimeOfDay;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (isDragging) DragEnd(this, EventArgs.Empty);
        else NonDragClick(this, EventArgs.Empty);


        isMouseDown = false;
        isDragging = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!isMouseDown) return;

        if (!isDragging)
        {
            isDragging = CheckIfDragging(e);

            if (isDragging) DragStart(this, EventArgs.Empty);
            else return;
        }

        Drag(this, new DragEventArgs(lastLocation, e.Location));
    }

    private bool CheckIfDragging(MouseEventArgs e)
    {
        // Check if the mouse has moved more than 10 pixels
        var dx = e.Location.X - lastLocation.X;
        var dy = e.Location.Y - lastLocation.Y;

        // Check if the mouse has been held for more than 100ms
        var timediff = DateTime.Now.TimeOfDay - clickTime;

        return dx + dy > 10 || timediff.TotalMilliseconds > 100;
    }
}