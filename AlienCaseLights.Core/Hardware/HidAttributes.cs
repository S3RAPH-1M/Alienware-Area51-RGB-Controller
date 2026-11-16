using System.Runtime.InteropServices;

namespace AlienCaseLights.Core.Hardware;

[StructLayout(LayoutKind.Sequential)]
internal struct HidAttributes
{
    public uint Size;

    public ushort VendorId;

    public ushort ProductId;

    public ushort VersionNumber;
}
