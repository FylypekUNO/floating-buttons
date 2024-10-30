namespace floating_buttons;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var overlayForm = new OverlayForm();

        overlayForm.AddKeystrokeButton("Ctrl+Z", "^z");
        overlayForm.AddKeystrokeButton("Ctrl+Y", "^y");

        Application.Run(overlayForm);
    }
}