using System.Runtime.InteropServices;

namespace AlienCaseLights.Core.Hardware;

[StructLayout(LayoutKind.Sequential)]
internal struct DeviceInterfaceData
{
    public uint Size;

    public Guid InterfaceClassGuid;

    public uint Flags;

    public IntPtr Reserved;
}
