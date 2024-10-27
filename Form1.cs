using System.Runtime.InteropServices;

namespace floating_buttons;

public partial class Form1 : Form
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_EXSTYLE = -20; // Extended window styles
    private const int WS_EX_LAYERED = 0x80000; // Allows transparency
    private const int WS_EX_TOOLWINDOW = 0x80; // Hides from Alt+Tab
    private const int WS_EX_NOACTIVATE = 0x8000000; // Prevents activation on click

    private CustomButton1 myButton;

    public String label { get; private set; }
    public String keystroke { get; private set; }

    public Form1(String label, String keystroke)
    {
        this.label = label;
        this.keystroke = keystroke;

        this.Load += OnLoad;

        InitilizeComponent();
        InitializeButton();
    }

    private void InitilizeComponent()
    {
        this.SuspendLayout();
        this.ClientSize = new Size(200, 200);
        this.Name = "Form1";
        this.ResumeLayout(false);
    }

    private void InitializeButton()
    {
        myButton = new CustomButton1(label);

        myButton.Drag += (sender, e) =>
        {
            this.Left += e.DeltaX;
            this.Top += e.DeltaY;
        };

        myButton.NonDragClick += (sender, e) => { SendKeys.SendWait(keystroke); };

        this.Controls.Add(myButton);
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        this.ShowInTaskbar = false; // Hide from taskbar
        this.FormBorderStyle = FormBorderStyle.None; // Remove window borders
        this.TopMost = true; // Keep the form on top
        this.WindowState = FormWindowState.Normal; // Set normal state
        this.BackColor = Color.Lime; // Set a background color (used for transparency)
        this.TransparencyKey = Color.Lime; // Make this color transparent
        this.AllowTransparency = true; // Enable transparency

        // Set the window styles to hide it and allow interaction
        var exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
        SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE);
    }
}