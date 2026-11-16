namespace AlienCaseLights.Core.Lights;

public sealed record LightZone(int ZoneId, int LedId, string Name, double Position)
{
    public uint LedMask => 1u << LedId;
}
