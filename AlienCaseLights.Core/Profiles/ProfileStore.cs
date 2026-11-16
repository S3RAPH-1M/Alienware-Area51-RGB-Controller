using System.Text.Json;

namespace AlienCaseLights.Core.Profiles;

public sealed class ProfileStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    private readonly string path;

    public ProfileStore(string path)
    {
        this.path = path;
    }

    public static string DefaultPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AlienCaseLights", "profile.json");

    public LightProfile Load()
    {
        try
        {
            LightProfile? profile = JsonSerializer.Deserialize<LightProfile>(File.ReadAllText(path), Options);

            if (profile is { ZoneColors.Length: > 0 })
            {
                return profile;
            }
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
        }

        return LightProfile.Default;
    }

    public void Save(LightProfile profile)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(profile, Options));
    }
}
