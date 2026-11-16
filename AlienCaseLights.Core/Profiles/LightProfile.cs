using AlienCaseLights.Core.Lights;

namespace AlienCaseLights.Core.Profiles;

public sealed record LightProfile(string EffectName, Rgb Primary, Rgb Secondary, double Speed, double Brightness, Rgb[] ZoneColors)
{
    public static LightProfile Default { get; } = new(
        "Rainbow Wave",
        new Rgb(0, 240, 240),
        new Rgb(0, 0, 240),
        1.0,
        1.0,
        Enumerable.Repeat(new Rgb(0, 240, 240), CaseLayout.Zones.Count).ToArray());
}
