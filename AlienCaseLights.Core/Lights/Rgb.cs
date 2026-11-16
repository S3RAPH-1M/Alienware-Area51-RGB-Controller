namespace AlienCaseLights.Core.Lights;

public readonly record struct Rgb(byte R, byte G, byte B)
{
    public static Rgb Black => new(0, 0, 0);

    public static Rgb White => new(255, 255, 255);

    public static Rgb Lerp(Rgb from, Rgb to, double amount)
    {
        amount = Math.Clamp(amount, 0, 1);

        return new Rgb(
            (byte)Math.Round(from.R + (to.R - from.R) * amount),
            (byte)Math.Round(from.G + (to.G - from.G) * amount),
            (byte)Math.Round(from.B + (to.B - from.B) * amount));
    }

    public static Rgb FromHsv(double hue, double saturation, double value)
    {
        hue = ((hue % 360) + 360) % 360;
        saturation = Math.Clamp(saturation, 0, 1);
        value = Math.Clamp(value, 0, 1);

        double chroma = value * saturation;
        double sector = hue / 60;
        double second = chroma * (1 - Math.Abs(sector % 2 - 1));
        double offset = value - chroma;

        (double r, double g, double b) = (int)sector switch
        {
            0 => (chroma, second, 0.0),
            1 => (second, chroma, 0.0),
            2 => (0.0, chroma, second),
            3 => (0.0, second, chroma),
            4 => (second, 0.0, chroma),
            _ => (chroma, 0.0, second)
        };

        return new Rgb(
            (byte)Math.Round((r + offset) * 255),
            (byte)Math.Round((g + offset) * 255),
            (byte)Math.Round((b + offset) * 255));
    }

    public (double Hue, double Saturation, double Value) ToHsv()
    {
        double r = R / 255.0;
        double g = G / 255.0;
        double b = B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double delta = max - Math.Min(r, Math.Min(g, b));

        double hue = 0;

        if (delta > 0)
        {
            hue = max == r ? 60 * (((g - b) / delta + 6) % 6) : max == g ? 60 * ((b - r) / delta + 2) : 60 * ((r - g) / delta + 4);
        }

        return (hue, max == 0 ? 0 : delta / max, max);
    }

    public Rgb Scale(double factor)
    {
        factor = Math.Clamp(factor, 0, 1);

        return new Rgb((byte)Math.Round(R * factor), (byte)Math.Round(G * factor), (byte)Math.Round(B * factor));
    }

    public Rgb Quantize()
    {
        return new Rgb(ToNibbleLevel(R), ToNibbleLevel(G), ToNibbleLevel(B));
    }

    private static byte ToNibbleLevel(byte channel)
    {
        return (byte)(((channel * 15 + 127) / 255) << 4);
    }
}
