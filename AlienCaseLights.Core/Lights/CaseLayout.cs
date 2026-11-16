namespace AlienCaseLights.Core.Lights;

public static class CaseLayout
{
    public static IReadOnlyList<LightZone> Zones { get; } =
    [
        new LightZone(1, 5, "Right Side 1", 0.125),
        new LightZone(2, 7, "Right Side 2", 0.247),
        new LightZone(3, 6, "Right Side 3", 0.0),
        new LightZone(4, 0, "Left Side 1", 0.947),
        new LightZone(5, 2, "Left Side 2", 1.0),
        new LightZone(6, 1, "Left Side 3", 0.888),
        new LightZone(7, 10, "Alienhead", 0.760),
        new LightZone(8, 11, "Front Left", 0.569),
        new LightZone(9, 12, "Front Right", 0.794)
    ];
}
