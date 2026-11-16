using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class RainbowWaveEffect : LightEffect
{
    public override string Name => "Rainbow Wave";

    public override bool UsesPrimary => false;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        for (int index = 0; index < frame.Length; index++)
        {
            frame[index] = Rgb.FromHsv(seconds * 45 * profile.Speed - CaseLayout.Zones[index].Position * 300, 1, 1);
        }
    }
}
