using System;
using Futronic.Devices.FS26;

namespace ReadMifareCardDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LibMifareApi Demo");

            var accessor = new DeviceAccessor();

            using (var device = accessor.AccessCardReader())
            {
                var sn = device.GetCardSerialNumber();

                Console.WriteLine($"Serial number: {sn:D}, 0x{sn:X}");

            }

            Console.ReadLine();
        }
    }
}