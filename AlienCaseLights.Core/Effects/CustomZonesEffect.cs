using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public sealed class CustomZonesEffect : LightEffect
{
    public override string Name => "Custom Zones";

    public override bool UsesPrimary => false;

    public override bool UsesSpeed => false;

    public override bool UsesZoneColors => true;

    public override void Render(double seconds, LightProfile profile, Rgb[] frame)
    {
        for (int index = 0; index < frame.Length; index++)
        {
            frame[index] = index < profile.ZoneColors.Length ? profile.ZoneColors[index] : Rgb.Black;
        }
    }
}
