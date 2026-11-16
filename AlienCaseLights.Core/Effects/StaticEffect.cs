using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class StaticEffect : LightEffect
{
    public override string Name => "Static";

    public override bool UsesSpeed => false;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        Array.Fill(frame, profile.Primary);
    }
}
