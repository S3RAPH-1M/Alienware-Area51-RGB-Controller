namespace AlienCaseLights.Core.Protocol;

public enum ResetMode : byte
{
    Legacy = 0,
    UserControlsLighted = 1,
    SleepLightsOnly = 2,
    AllLightsOff = 3,
    LeaveLightsOn = 4,
    PreserveLightingData = 5
}
