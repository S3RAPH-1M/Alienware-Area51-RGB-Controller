namespace AlienCaseLights.Core.Protocol;

public enum DeviceStatus : byte
{
    Ready = 16,
    Busy = 17,
    UnsupportedCommand = 18,
    NoFirmwarePresent = 152,
    Error = 153,
    WaitingForPowerState = 240
}
