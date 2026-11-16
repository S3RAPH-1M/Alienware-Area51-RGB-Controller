using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using AlienCaseLights.Core.Lights;

namespace AlienCaseLights.App.Controls;

public sealed class ColorWheel : Control
{
    public static readonly StyledProperty<double> HueProperty =
        AvaloniaProperty.Register<ColorWheel, double>(nameof(Hue), 0, false, BindingMode.TwoWay);

    public static readonly StyledProperty<double> SaturationProperty =
        AvaloniaProperty.Register<ColorWheel, double>(nameof(Saturation), 1, false, BindingMode.TwoWay);

    private const double ThumbRadius = 9;

    static ColorWheel()
    {
        AffectsRender<ColorWheel>(HueProperty, SaturationProperty);
    }

    public double Hue
    {
        get => GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    public double Saturation
    {
        get => GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        double diameter = Math.Min(Bounds.Width, Bounds.Height);
        Point center = new(Bounds.Width / 2, Bounds.Height / 2);
        double radius = diameter / 2 - ThumbRadius;

        context.DrawImage(
            ColorWheelBitmap.Get(),
            new Rect(0, 0, ColorWheelBitmap.Size, ColorWheelBitmap.Size),
            new Rect(center.X - radius, center.Y - radius, radius * 2, radius * 2));

        double angle = Hue * Math.PI / 180;
        Point thumb = new(center.X + Math.Cos(angle) * Saturation * radius, center.Y + Math.Sin(angle) * Saturation * radius);
        Rgb color = Rgb.FromHsv(Hue, Saturation, 1);

        context.DrawEllipse(new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B)), new Pen(Brushes.White, 3), thumb, ThumbRadius, ThumbRadius);
        context.DrawEllipse(null, new Pen(new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)), 1), thumb, ThumbRadius + 1.5, ThumbRadius + 1.5);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double side = Math.Min(double.IsInfinity(availableSize.Width) ? 240 : availableSize.Width, double.IsInfinity(availableSize.Height) ? 240 : availableSize.Height);

        return new Size(side, side);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        e.Pointer.Capture(this);
        Pick(e.GetPosition(this));
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (ReferenceEquals(e.Pointer.Captured, this))
        {
            Pick(e.GetPosition(this));
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        e.Pointer.Capture(null);
    }

    private void Pick(Point position)
    {
        double diameter = Math.Min(Bounds.Width, Bounds.Height);
        double radius = diameter / 2 - ThumbRadius;
        double dx = position.X - Bounds.Width / 2;
        double dy = position.Y - Bounds.Height / 2;

        Hue = (Math.Atan2(dy, dx) * 180 / Math.PI + 360) % 360;
        Saturation = Math.Min(1, Math.Sqrt(dx * dx + dy * dy) / radius);
    }
}
