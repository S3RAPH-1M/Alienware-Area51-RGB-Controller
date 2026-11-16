using Avalonia.Media;
using AlienCaseLights.Core.Lights;

namespace AlienCaseLights.App.ViewModels;

public sealed class ZoneViewModel : ViewModelBase
{
    private IBrush live = Brushes.Black;

    private bool isSelected;

    public ZoneViewModel(int index, LightZone zone, Rgb customColor)
    {
        Index = index;
        Name = zone.Name;
        Position = zone.Position;
        CustomColor = customColor;
    }

    public int Index { get; }

    public string Name { get; }

    public double Position { get; }

    public Rgb CustomColor { get; set; }

    public IBrush Outline => isSelected ? Brushes.White : Brushes.Transparent;

    public IBrush Live
    {
        get => live;
        set => SetProperty(ref live, value);
    }

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (SetProperty(ref isSelected, value))
            {
                Raise(nameof(Outline));
            }
        }
    }
}
