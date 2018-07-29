using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Futronic.Devices.FS26;

namespace CombinedDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Accessing device");
            var da = new DeviceAccessor();

            var fp = da.AccessFingerprintDevice();
            var cr = da.AccessCardReader();

            fp.FingerDetected += (sender, eventArgs) => Console.WriteLine("Finger detected!");
            cr.CardDetected += (sender, eventArgs) => Console.WriteLine($"Card detected: {eventArgs.SerialNumber:X10}");

            fp.FingerReleased += (sender, eventArgs) => Console.WriteLine("Finger released");
            cr.CardRemoved += (sender, eventArgs) => Console.WriteLine("Card removed");

            fp.StartFingerDetection();
            cr.StartCardDetection();
            Console.WriteLine("Detection of FP and Card has started.");

            Console.ReadLine();

            Console.WriteLine("Closing device. Please wait");
            fp.Dispose();
            cr.Dispose();
        }
    }
}
