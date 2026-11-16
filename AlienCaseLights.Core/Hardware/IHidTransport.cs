namespace AlienCaseLights.Core.Hardware;

public interface IHidTransport : IDisposable
{
    bool IsOpen { get; }

    bool Open(ushort vendorId, ushort productId);

    bool SendReport(byte[] report);

    bool ReceiveReport(byte[] report);
}
