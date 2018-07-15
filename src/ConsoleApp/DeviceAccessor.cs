using System;

namespace ConsoleApp1
{
    class DeviceAccessor
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