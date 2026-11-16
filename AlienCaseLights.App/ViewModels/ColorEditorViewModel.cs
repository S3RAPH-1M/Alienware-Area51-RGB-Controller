using Avalonia.Media;
using AlienCaseLights.Core.Lights;

namespace AlienCaseLights.App.ViewModels;

public sealed class ColorEditorViewModel : ViewModelBase
{
    private double hue;

    private double saturation;

    private double brightness;

    public ColorEditorViewModel(string title, Rgb initial)
    {
        Title = title;
        Load(initial);
    }

    public event Action? Changed;

    public string Title { get; }

    public double Hue
    {
        get => hue;
        set => Edit(ref hue, value);
    }

    public double Saturation
    {
        get => saturation;
        set => Edit(ref saturation, value);
    }

    public double Brightness
    {
        get => brightness;
        set => Edit(ref brightness, value);
    }

    public Rgb Color => Rgb.FromHsv(hue, saturation, brightness);

    public IBrush Preview
    {
        get
        {
            Rgb color = Color;

            return new SolidColorBrush(Avalonia.Media.Color.FromRgb(color.R, color.G, color.B));
        }
    }

    public void Load(Rgb color)
    {
        (double newHue, double newSaturation, double newBrightness) = color.ToHsv();

        hue = newHue;
        saturation = newSaturation;
        brightness = newBrightness;

        RaiseAll();
    }

    private void Edit(ref double field, double value)
    {
        if (field == value)
        {
            return;
        }

        field = value;

        RaiseAll();
        Changed?.Invoke();
    }

    private void RaiseAll()
    {
        Raise(nameof(Hue));
        Raise(nameof(Saturation));
        Raise(nameof(Brightness));
        Raise(nameof(Preview));
    }
}
