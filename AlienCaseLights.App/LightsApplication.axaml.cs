using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AlienCaseLights.App.Services;
using AlienCaseLights.App.ViewModels;
using AlienCaseLights.Core.Devices;
using AlienCaseLights.Core.Effects;
using AlienCaseLights.Core.Hardware;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.App;

public partial class LightsApplication : Application
{
    private IClassicDesktopStyleApplicationLifetime? desktop;

    private Mutex? instanceLock;

    private EffectRunner? runner;

    private MainViewModel? model;

    private MainWindow? window;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        desktop = ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        if (desktop is not null)
        {
            instanceLock = new Mutex(true, "AlienCaseLights.SingleInstance", out bool isFirst);

            if (!isFirst)
            {
                desktop.Shutdown();
                base.OnFrameworkInitializationCompleted();

                return;
            }

            ProfileStore store = new(ProfileStore.DefaultPath);

            runner = new EffectRunner(new CaseController(new HidTransport()), store.Load());
            model = new MainViewModel(runner, store, new StartupRegistration());
            window = new MainWindow { DataContext = model };

            desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnExplicitShutdown;
            desktop.Exit += (_, _) => Cleanup();

            runner.Start();

            if (!desktop.Args!.Contains("--minimized"))
            {
                window.Show();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnTrayClicked(object? sender, EventArgs e)
    {
        ShowWindow();
    }

    private void OnShowClicked(object? sender, EventArgs e)
    {
        ShowWindow();
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        window?.AllowClose();
        desktop?.Shutdown();
    }

    private void ShowWindow()
    {
        window?.Show();
        window?.Activate();
    }

    private void Cleanup()
    {
        model?.Save();
        runner?.Dispose();
        instanceLock?.Dispose();
    }
}
