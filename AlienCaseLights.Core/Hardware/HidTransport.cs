using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace AlienCaseLights.Core.Hardware;

public sealed class HidTransport : IHidTransport
{
    private readonly object gate = new();

    private SafeFileHandle? handle;

    public bool IsOpen => handle is { IsInvalid: false, IsClosed: false };

    public bool Open(ushort vendorId, ushort productId)
    {
        lock (gate)
        {
            Close();

            string? path = FindPath(vendorId, productId);

            if (path is null)
            {
                return false;
            }

            SafeFileHandle opened = HidNative.CreateFileW(path, HidNative.GenericRead | HidNative.GenericWrite, HidNative.ShareReadWrite, IntPtr.Zero, HidNative.OpenExisting, 0, IntPtr.Zero);

            if (opened.IsInvalid)
            {
                opened.Dispose();

                return false;
            }

            handle = opened;

            return true;
        }
    }

    public bool SendReport(byte[] report)
    {
        lock (gate)
        {
            if (!IsOpen)
            {
                return false;
            }

            bool sent = HidNative.HidD_SetOutputReport(handle!, report, (uint)report.Length);

            if (!sent)
            {
                Close();
            }

            return sent;
        }
    }

    public bool ReceiveReport(byte[] report)
    {
        lock (gate)
        {
            return IsOpen && HidNative.HidD_GetInputReport(handle!, report, (uint)report.Length);
        }
    }

    public void Dispose()
    {
        lock (gate)
        {
            Close();
        }
    }

    private void Close()
    {
        handle?.Dispose();
        handle = null;
    }

    private static string? FindPath(ushort vendorId, ushort productId)
    {
        HidNative.HidD_GetHidGuid(out Guid hidGuid);

        IntPtr deviceSet = HidNative.SetupDiGetClassDevsW(ref hidGuid, IntPtr.Zero, IntPtr.Zero, HidNative.DigcfPresent | HidNative.DigcfDeviceInterface);

        if (deviceSet == HidNative.InvalidHandle)
        {
            return null;
        }

        try
        {
            DeviceInterfaceData data = new() { Size = (uint)Marshal.SizeOf<DeviceInterfaceData>() };

            for (uint index = 0; HidNative.SetupDiEnumDeviceInterfaces(deviceSet, IntPtr.Zero, ref hidGuid, index, ref data); index++)
            {
                string? path = ReadPath(deviceSet, ref data);

                if (path is not null && Matches(path, vendorId, productId))
                {
                    return path;
                }
            }

            return null;
        }
        finally
        {
            HidNative.SetupDiDestroyDeviceInfoList(deviceSet);
        }
    }

    private static string? ReadPath(IntPtr deviceSet, ref DeviceInterfaceData data)
    {
        HidNative.SetupDiGetDeviceInterfaceDetailW(deviceSet, ref data, IntPtr.Zero, 0, out uint required, IntPtr.Zero);

        if (required == 0)
        {
            return null;
        }

        IntPtr buffer = Marshal.AllocHGlobal((int)required);

        try
        {
            Marshal.WriteInt32(buffer, IntPtr.Size == 8 ? 8 : 6);

            if (!HidNative.SetupDiGetDeviceInterfaceDetailW(deviceSet, ref data, buffer, required, out _, IntPtr.Zero))
            {
                return null;
            }

            return Marshal.PtrToStringUni(buffer + 4);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private static bool Matches(string path, ushort vendorId, ushort productId)
    {
        using SafeFileHandle probe = HidNative.CreateFileW(path, 0, HidNative.ShareReadWrite, IntPtr.Zero, HidNative.OpenExisting, 0, IntPtr.Zero);

        if (probe.IsInvalid)
        {
            return false;
        }

        HidAttributes attributes = new() { Size = (uint)Marshal.SizeOf<HidAttributes>() };

        return HidNative.HidD_GetAttributes(probe, ref attributes) && attributes.VendorId == vendorId && attributes.ProductId == productId;
    }
}
