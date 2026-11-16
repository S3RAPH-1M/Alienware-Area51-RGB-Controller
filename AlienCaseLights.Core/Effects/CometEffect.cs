using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class CometEffect : LightEffect
{
    public override string Name => "Comet";

    public override bool UsesSecondary => true;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        double head = (seconds * profile.Speed * 0.3) % 1.4 - 0.2;

        for (int index = 0; index < frame.Length; index++)
        {
            double distance = head - CaseLayout.Zones[index].Position;
            double strength = distance < 0 ? Math.Exp(distance * 25) : Math.Exp(-distance * 4);

            frame[index] = Rgb.Lerp(profile.Secondary, profile.Primary, strength);
        }
    }
}
