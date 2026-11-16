using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class ColorCycleEffect : LightEffect
{
    public override string Name => "Color Cycle";

    public override bool UsesPrimary => false;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        Array.Fill(frame, Rgb.FromHsv(seconds * 45 * profile.Speed, 1, 1));
    }
}
