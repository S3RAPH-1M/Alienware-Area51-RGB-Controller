using AlienCaseLights.Core.Lights;

namespace AlienCaseLights.Core.Protocol;

public static class PacketBuilder
{
    public const int ReportLength = 9;

    public const byte ReportId = 2;

    public static byte[] Command(CommandId command, byte argument = 0)
    {
        byte[] packet = new byte[ReportLength];
        packet[0] = ReportId;
        packet[1] = (byte)command;
        packet[2] = argument;

        return packet;
    }

    public static byte[] Reset(ResetMode mode)
    {
        return Command(CommandId.Reset, (byte)mode);
    }

    public static byte[] Start(bool overridePowerStatus = false)
    {
        return Command(CommandId.Start, overridePowerStatus ? (byte)1 : (byte)0);
    }

    public static byte[] Loop()
    {
        return Command(CommandId.Loop);
    }

    public static byte[] Timing(int tempo, int shortActionMilliseconds)
    {
        byte[] packet = Command(CommandId.SetTempo);
        packet[2] = (byte)(tempo >> 8);
        packet[3] = (byte)tempo;
        packet[4] = (byte)(shortActionMilliseconds >> 8);
        packet[5] = (byte)shortActionMilliseconds;

        return packet;
    }

    public static byte[] Action(CommandId kind, LightZone zone, Rgb first, Rgb second, bool longDuration = false)
    {
        uint mask = zone.LedMask;

        byte[] packet = new byte[ReportLength];
        packet[0] = ReportId;
        packet[1] = (byte)kind;
        packet[2] = (byte)(zone.ZoneId | (longDuration ? 0x80 : 0));
        packet[3] = (byte)((mask >> 16) & 0x0F);
        packet[4] = (byte)((mask >> 8) & 0xFF);
        packet[5] = (byte)(mask & 0xFF);
        packet[6] = (byte)((first.R & 0xF0) | (first.G >> 4));
        packet[7] = (byte)((first.B & 0xF0) | (second.R >> 4));
        packet[8] = (byte)((second.G & 0xF0) | (second.B >> 4));

        return packet;
    }

    public static byte[] SetColor(LightZone zone, Rgb color)
    {
        return Action(CommandId.SetColor, zone, color, Rgb.Black);
    }
}
