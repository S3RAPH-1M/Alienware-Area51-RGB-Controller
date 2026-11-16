using AlienCaseLights.Core.Hardware;
using AlienCaseLights.Core.Lights;
using AlienCaseLights.Core.Protocol;

namespace AlienCaseLights.Core.Devices;

public sealed class CaseController : IDisposable
{
    public const ushort VendorId = 0x187C;

    public const ushort ProductId = 0x0526;

    private const int TempoMilliseconds = 1000;

    private const int ActionMilliseconds = 1000;

    private const int ReadyPollLimit = 2000;

    private readonly IHidTransport transport;

    public CaseController(IHidTransport transport)
    {
        this.transport = transport;
    }

    public bool IsConnected => transport.IsOpen;

    public bool Connect()
    {
        return transport.IsOpen || transport.Open(VendorId, ProductId);
    }

    public bool ShowFrame(IReadOnlyList<Rgb> colors)
    {
        if (!Connect())
        {
            return false;
        }

        bool sent = transport.SendReport(PacketBuilder.Reset(ResetMode.LeaveLightsOn)) && WaitUntilReady();

        sent = sent && transport.SendReport(PacketBuilder.Timing(TempoMilliseconds, ActionMilliseconds));

        IReadOnlyList<LightZone> zones = CaseLayout.Zones;

        for (int zone = 0; sent && zone < zones.Count; zone++)
        {
            sent = transport.SendReport(PacketBuilder.SetColor(zones[zone], colors[zone])) && transport.SendReport(PacketBuilder.Loop());
        }

        return sent && transport.SendReport(PacketBuilder.Start());
    }

    public void Dispose()
    {
        transport.Dispose();
    }

    private bool WaitUntilReady()
    {
        byte[] status = new byte[PacketBuilder.ReportLength];

        for (int poll = 0; poll < ReadyPollLimit; poll++)
        {
            status[0] = 1;

            if (transport.SendReport(PacketBuilder.Command(CommandId.GetStatus)) && transport.ReceiveReport(status) && status[0] == (byte)DeviceStatus.Ready)
            {
                return true;
            }

            Thread.Sleep(1);
        }

        return false;
    }
}
