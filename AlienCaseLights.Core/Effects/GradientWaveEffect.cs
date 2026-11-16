using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class GradientWaveEffect : LightEffect
{
    public override string Name => "Gradient Wave";

    public override bool UsesSecondary => true;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        for (int index = 0; index < frame.Length; index++)
        {
            double phase = CaseLayout.Zones[index].Position - seconds * profile.Speed * 0.25;
            double amount = 0.5 + 0.5 * Math.Sin(2 * Math.PI * phase);

            frame[index] = Rgb.Lerp(profile.Secondary, profile.Primary, amount);
        }
    }
}
