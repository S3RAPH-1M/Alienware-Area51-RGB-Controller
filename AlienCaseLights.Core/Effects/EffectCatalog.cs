namespace AlienCaseLights.Core.Effects;

public static class EffectCatalog
{
    public static IReadOnlyList<LightEffect> All { get; } =
    [
        new StaticEffect(),
        new CustomZonesEffect(),
        new BreathingEffect(),
        new ColorCycleEffect(),
        new RainbowWaveEffect(),
        new GradientWaveEffect(),
        new CometEffect(),
        new StarlightEffect(),
        new StrobeEffect(),
        new FireEffect()
    ];

    public static LightEffect Find(string name)
    {
        return All.FirstOrDefault(effect => effect.Name == name) ?? All[0];
    }
}
