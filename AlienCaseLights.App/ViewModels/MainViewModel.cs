using System.Diagnostics;
using System.Windows.Input;
using Avalonia.Media;
using Avalonia.Threading;
using AlienCaseLights.App.Services;
using AlienCaseLights.Core.Effects;
using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.App.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly EffectRunner runner;

    private readonly ProfileStore store;

    private readonly StartupRegistration startup;

    private readonly DispatcherTimer saveTimer;

    private readonly DispatcherTimer previewTimer;

    private readonly Stopwatch previewClock = Stopwatch.StartNew();

    private readonly ZoneViewModel[] zonesByIndex;

    private readonly Rgb[] previewFrame = new Rgb[CaseLayout.Zones.Count];

    private LightProfile profile;

    private LightEffect selectedEffect;

    private ZoneViewModel selectedZone;

    private double speed;

    private double brightness;

    private bool lightsOn = true;

    private bool isConnected;

    public MainViewModel(EffectRunner runner, ProfileStore store, StartupRegistration startup)
    {
        this.runner = runner;
        this.store = store;
        this.startup = startup;

        profile = runner.Profile;
        selectedEffect = EffectCatalog.Find(profile.EffectName);
        speed = profile.Speed;
        brightness = profile.Brightness;

        Primary = new ColorEditorViewModel("Color", profile.Primary);
        Secondary = new ColorEditorViewModel("Secondary Color", profile.Secondary);
        ZoneEditor = new ColorEditorViewModel("Zone Color", Rgb.Black);

        zonesByIndex = CaseLayout.Zones
            .Select((zone, index) => new ZoneViewModel(index, zone, index < profile.ZoneColors.Length ? profile.ZoneColors[index] : profile.Primary))
            .ToArray();

        Zones = zonesByIndex.OrderBy(zone => zone.Position).ToArray();
        selectedZone = Zones[0];
        selectedZone.IsSelected = true;
        ZoneEditor.Load(selectedZone.CustomColor);

        SelectZoneCommand = new RelayCommand<ZoneViewModel>(SelectZone);

        Primary.Changed += Apply;
        Secondary.Changed += Apply;
        ZoneEditor.Changed += ApplyZoneColor;

        saveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        saveTimer.Tick += (_, _) => Save();

        previewTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        previewTimer.Tick += (_, _) => UpdatePreview();
        previewTimer.Start();

        runner.ConnectionChanged += OnConnectionChanged;
        isConnected = runner.IsConnected;
    }

    public IReadOnlyList<LightEffect> Effects => EffectCatalog.All;

    public IReadOnlyList<ZoneViewModel> Zones { get; }

    public ColorEditorViewModel Primary { get; }

    public ColorEditorViewModel Secondary { get; }

    public ColorEditorViewModel ZoneEditor { get; }

    public ICommand SelectZoneCommand { get; }

    public LightEffect SelectedEffect
    {
        get => selectedEffect;
        set
        {
            if (value is not null && SetProperty(ref selectedEffect, value))
            {
                Raise(nameof(ShowPrimary));
                Raise(nameof(ShowSecondary));
                Raise(nameof(ShowSpeed));
                Raise(nameof(ShowZones));
                Apply();
            }
        }
    }

    public double Speed
    {
        get => speed;
        set
        {
            if (SetProperty(ref speed, value))
            {
                Apply();
            }
        }
    }

    public double Brightness
    {
        get => brightness;
        set
        {
            if (SetProperty(ref brightness, value))
            {
                Apply();
            }
        }
    }

    public bool LightsOn
    {
        get => lightsOn;
        set
        {
            if (SetProperty(ref lightsOn, value))
            {
                runner.Enabled = value;
            }
        }
    }

    public bool StartWithWindows
    {
        get => startup.IsEnabled;
        set
        {
            startup.IsEnabled = value;
            Raise(nameof(StartWithWindows));
        }
    }

    public bool ShowPrimary => selectedEffect.UsesPrimary;

    public bool ShowSecondary => selectedEffect.UsesSecondary;

    public bool ShowSpeed => selectedEffect.UsesSpeed;

    public bool ShowZones => selectedEffect.UsesZoneColors;

    public string Status => isConnected ? "Case connected" : "Case not found";

    public void Save()
    {
        saveTimer.Stop();
        store.Save(profile);
    }

    private void Apply()
    {
        profile = new LightProfile(
            selectedEffect.Name,
            Primary.Color,
            Secondary.Color,
            speed,
            brightness,
            zonesByIndex.Select(zone => zone.CustomColor).ToArray());

        runner.Profile = profile;

        saveTimer.Stop();
        saveTimer.Start();
    }

    private void ApplyZoneColor()
    {
        selectedZone.CustomColor = ZoneEditor.Color;
        Apply();
    }

    private void SelectZone(ZoneViewModel zone)
    {
        selectedZone.IsSelected = false;
        selectedZone = zone;
        selectedZone.IsSelected = true;
        ZoneEditor.Load(zone.CustomColor);
    }

    private void UpdatePreview()
    {
        EffectRunner.Compose(selectedEffect, profile, previewClock.Elapsed.TotalSeconds, lightsOn, previewFrame);

        foreach (ZoneViewModel zone in zonesByIndex)
        {
            Rgb color = previewFrame[zone.Index];

            zone.Live = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
        }
    }

    private void OnConnectionChanged(bool connected)
    {
        Dispatcher.UIThread.Post(() =>
        {
            isConnected = connected;
            Raise(nameof(Status));
        });
    }
}
