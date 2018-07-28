using System;
using System.Runtime.InteropServices;

namespace Futronic.Devices.FS26
{
    static class LibMilfareApi
    {
        [DllImport("ftrMFAPI.dll")]
        internal static extern IntPtr ftrMFOpenDevice();

        [DllImport("ftrMFAPI.dll")]
        internal static extern void ftrMFCloseDevice(IntPtr handle);

        [DllImport("ftrMFAPI.dll")]
        internal static extern bool ftrMFStartSequence(IntPtr handle);

        [DllImport("ftrMFAPI.dll")]
        internal static extern bool ftrMFEndSequence(IntPtr handle);

        [DllImport("ftrMFAPI.dll")]
        internal static extern bool ftrMFActivateIdle8bSN(IntPtr handle, byte[] cardType, byte[] serialNumber);
    }
}
