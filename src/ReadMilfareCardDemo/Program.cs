using System;
using Futronic.Devices.FS26;

namespace ReadMilfareCardDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LibMilfareApi Demo");

            var accessor = new DeviceAccessor();

            using (var device = accessor.OpenCardReader())
            {
                var sn = device.GetCardSerialNumber();

                Console.WriteLine($"Serial number: {sn:D}, 0x{sn:X}");

            }

            Console.ReadLine();
        }
    }
}
