namespace AlienCaseLights.Core.Protocol;

public enum CommandId : byte
{
    Morph = 1,
    Pulse = 2,
    SetColor = 3,
    Loop = 4,
    Start = 5,
    GetStatus = 6,
    Reset = 7,
    SetTempo = 14
}
