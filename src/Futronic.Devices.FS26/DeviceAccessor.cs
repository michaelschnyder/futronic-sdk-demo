using System;

namespace Futronic.Devices.FS26
{
    public class DeviceAccessor
    {
        public Device Open()
        {
            var handle = LibScanApi.ftrScanOpenDevice();

            if (handle != IntPtr.Zero)
            {
                return new Device(handle);
            }

            throw new Exception("Cannot open device");
        }
    }
}