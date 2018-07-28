using System;
using System.IO;
using Futronic.Devices.FS26;

namespace ReadFingerprintDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LibScanApi Demo");

            var accessor = new DeviceAccessor();

            using (var device = accessor.AccessFingerprintDevice())
            {
                device.SwitchLedState(false, false);

                device.FingerDetected += (sender, eventArgs) =>
                {
                    Console.WriteLine("Finger Detected!");

                    device.SwitchLedState(true, false);

                    // Save fingerprint to temporary folder
                    var fingerprint = device.ReadFingerprint();
                    var tempFile = Path.GetTempFileName();
                    var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                    fingerprint.Save(tmpBmpFile);

                    Console.WriteLine("Saved to " + tmpBmpFile);
                };

                device.FingerReleased += (sender, eventArgs) =>
                {
                    Console.WriteLine("Finger Released!");

                    device.SwitchLedState(false, true);
                };

                Console.WriteLine("FingerprintDevice Opened");

                device.StartFingerDetection();
                device.SwitchLedState(false, true);

                Console.ReadLine();

                Console.WriteLine("Exiting...");

                device.SwitchLedState(false, false);
            }
        }
    }
}
