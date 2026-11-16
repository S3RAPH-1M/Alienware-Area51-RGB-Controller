using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AlienCaseLights.Core.Lights;

namespace AlienCaseLights.App.Controls;

public static class ColorWheelBitmap
{
    public const int Size = 512;

    private static WriteableBitmap? cached;

    public static WriteableBitmap Get()
    {
        return cached ??= Create();
    }

    private static WriteableBitmap Create()
    {
        byte[] pixels = new byte[Size * Size * 4];
        double radius = Size / 2.0;

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                double dx = x + 0.5 - radius;
                double dy = y + 0.5 - radius;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                double coverage = Math.Clamp(radius - distance, 0, 1);

                if (coverage <= 0)
                {
                    continue;
                }

                double hue = Math.Atan2(dy, dx) * 180 / Math.PI;
                Rgb color = Rgb.FromHsv(hue, distance / radius, 1);
                int offset = (y * Size + x) * 4;

                pixels[offset] = (byte)Math.Round(color.B * coverage);
                pixels[offset + 1] = (byte)Math.Round(color.G * coverage);
                pixels[offset + 2] = (byte)Math.Round(color.R * coverage);
                pixels[offset + 3] = (byte)Math.Round(255 * coverage);
            }
        }

        WriteableBitmap bitmap = new(new PixelSize(Size, Size), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);

        using ILockedFramebuffer buffer = bitmap.Lock();

        for (int y = 0; y < Size; y++)
        {
            System.Runtime.InteropServices.Marshal.Copy(pixels, y * Size * 4, buffer.Address + y * buffer.RowBytes, Size * 4);
        }

        return bitmap;
    }
}
