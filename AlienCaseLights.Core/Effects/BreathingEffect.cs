using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class BreathingEffect : LightEffect
{
    public override string Name => "Breathing";

    public override bool UsesSecondary => true;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        double amount = 0.5 - 0.5 * Math.Cos(2 * Math.PI * seconds * profile.Speed / 4.0);

        Array.Fill(frame, Rgb.Lerp(profile.Secondary, profile.Primary, amount));
    }
}
