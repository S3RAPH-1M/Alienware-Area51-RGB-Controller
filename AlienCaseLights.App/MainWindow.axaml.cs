using System.ComponentModel;
using Avalonia.Controls;

namespace AlienCaseLights.App;

public partial class MainWindow : Window
{
    private bool closeAllowed;

    public MainWindow()
    {
        InitializeComponent();
    }

    public void AllowClose()
    {
        closeAllowed = true;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!closeAllowed)
        {
            e.Cancel = true;
            Hide();
        }

        base.OnClosing(e);
    }
}
