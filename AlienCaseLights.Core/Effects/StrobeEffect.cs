using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class StrobeEffect : LightEffect
{
    public override string Name => "Strobe";

    public override bool UsesSecondary => true;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        bool on = (int)(seconds * profile.Speed * 2) % 2 == 0;

        Array.Fill(frame, on ? profile.Primary : profile.Secondary);
    }
}
