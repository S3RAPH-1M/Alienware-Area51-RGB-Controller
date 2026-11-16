using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class FireEffect : LightEffect
{
    private static readonly Rgb Ember = new(240, 0, 0);

    private static readonly Rgb Flame = new(240, 112, 0);

    public override string Name => "Fire";

    public override bool UsesPrimary => false;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        double time = seconds * profile.Speed;

        for (int index = 0; index < frame.Length; index++)
        {
            double flicker = 0.5 + 0.25 * Math.Sin(time * 3.3 + index * 1.7) + 0.25 * Math.Sin(time * 1.9 + index * 2.9);

            frame[index] = Rgb.Lerp(Ember, Flame, flicker).Scale(0.35 + 0.65 * flicker);
        }
    }
}
