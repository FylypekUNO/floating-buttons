using System.Runtime.InteropServices;

namespace floating_buttons;

public class ButtonAndData
{
    public readonly string Label;
    public readonly string Keystroke;
    public readonly KeystrokeButton Button;

    public ButtonAndData(string label, string keystroke, KeystrokeButton button)
    {
        Label = label;
        Keystroke = keystroke;
        Button = button;
    }
}

public partial class OverlayForm : Form
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_EXSTYLE = -20; // Extended window styles
    private const int WS_EX_LAYERED = 0x80000; // Allows transparency
    private const int WS_EX_TOOLWINDOW = 0x80; // Hides from Alt+Tab
    private const int WS_EX_NOACTIVATE = 0x8000000; // Prevents activation on click

    public List<ButtonAndData> KeystrokeButtons { get; private set; } = new List<ButtonAndData>();

    private NotifyIcon _trayIcon;
    private ContextMenuStrip _trayMenu;
    private bool _buttonsEnabled = true;

    public OverlayForm()
    {
        SetVisuals();
        InitializeTrayIcon();
        OnDisplaySizeChange();
    }

    private void InitializeTrayIcon()
    {
        _trayMenu = new ContextMenuStrip();

        _trayMenu.Items.Add("Disable Buttons", null, (sender, e) => OnToggleButtons());
        _trayMenu.Items.Add(new ToolStripSeparator());
        _trayMenu.Items.Add("Control Panel", null, (sender, e) => OnOpenControlPanel());
        _trayMenu.Items.Add(new ToolStripSeparator());
        _trayMenu.Items.Add("Exit", null, (sender, e) => OnExit());

        _trayIcon = new NotifyIcon
        {
            Text = "Floating Buttons",
            Icon = SystemIcons.Application, // Template
            ContextMenuStrip = _trayMenu,
            Visible = true
        };

        _trayIcon.MouseDoubleClick += (sender, e) =>
        {
            if (e.Button != MouseButtons.Left) return;
            OnToggleButtons();
        };
    }

    private void OnToggleButtons()
    {
        _buttonsEnabled = !_buttonsEnabled;

        foreach (var btnData in KeystrokeButtons)
        {
            btnData.Button.Enabled = _buttonsEnabled;
            btnData.Button.Visible = _buttonsEnabled;
        }
    }

    private void OnOpenControlPanel()
    {
    }

    private void OnExit()
    {
        _trayIcon.Visible = false;
        Application.Exit();
    }

    public void AddKeystrokeButton(string label, string keystroke)
    {
        var myButton = new SingleKeystrokeButton(label, keystroke)
        {
            Location = new Point(100, 100)
        };

        this.Controls.Add(myButton);
        KeystrokeButtons.Add(new ButtonAndData(label, keystroke, myButton));
    }

    private void SetVisuals()
    {
        this.SuspendLayout();
        this.Name = "OverlayForm";
        this.ResumeLayout(false);

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

    protected override void WndProc(ref Message m)
    {
        const int WM_DISPLAYCHANGE = 0x007E;

        if (m.Msg == WM_DISPLAYCHANGE) OnDisplaySizeChange();

        base.WndProc(ref m);
    }

    private void OnDisplaySizeChange()
    {
        var screenBounds = Screen.PrimaryScreen.Bounds;

        var scaleX = (float)screenBounds.Width / this.Width;
        var scaleY = (float)screenBounds.Height / this.Height;

        // Fullscreen
        this.Location = new Point(0, 0);
        this.Size = new Size(screenBounds.Width, screenBounds.Height);

        // Scale controls positions proportionally
        foreach (Control control in this.Controls)
        {
            var controlCenterX = control.Left + control.Width / 2;
            var controlCenterY = control.Top + control.Height / 2;

            control.Left = (int)(controlCenterX * scaleX - control.Width / 2);
            control.Top = (int)(controlCenterY * scaleY - control.Height / 2);
        }
    }
}