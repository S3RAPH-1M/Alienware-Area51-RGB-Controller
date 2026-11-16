using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class StarlightEffect : LightEffect
{
    public override string Name => "Starlight";

    public override bool UsesSecondary => true;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        for (int index = 0; index < frame.Length; index++)
        {
            double rate = 0.2 + 0.1 * ((index * 5) % 7);
            double wave = Math.Sin(2 * Math.PI * (seconds * profile.Speed * rate + index * 0.37));

            frame[index] = Rgb.Lerp(profile.Secondary, profile.Primary, Math.Pow(Math.Max(0, wave), 3));
        }
    }
}
