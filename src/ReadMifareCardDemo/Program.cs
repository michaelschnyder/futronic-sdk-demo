using System;
using Futronic.Devices.FS26;

namespace ReadMifareCardDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LibMifareApi Demo");

            Console.WriteLine($"Last error {LibMifareApi.ftrMFGetLastError()}");

            var accessor = new DeviceAccessor();

            using (var device = accessor.AccessCardReader())
            {
                device.CardDetected += (sender, eventArgs) =>
                {
                    Console.WriteLine($"Card available. Type: {eventArgs.Type.ToString()}, SN: {eventArgs.SerialNumber:D}, 0x{eventArgs.SerialNumber:X8}");
                };

                device.CardRemoved += (sender, eventArgs) => Console.WriteLine("Card removed");

                device.StartCardDetection();
                Console.WriteLine("Card detection has started, please place card on device or hit enter to quit");

                Console.ReadLine();
            }
        }
    }
}