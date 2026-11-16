using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Profiles;

namespace AlienCaseLights.Core.Effects;

public abstract class LightEffect
{
    public abstract string Name { get; }

    public virtual bool UsesPrimary => true;

    public virtual bool UsesSecondary => false;

    public virtual bool UsesSpeed => true;

    public virtual bool UsesZoneColors => false;

    public abstract void Render(double seconds, LightProfile profile, Rgb[] frame);
}
