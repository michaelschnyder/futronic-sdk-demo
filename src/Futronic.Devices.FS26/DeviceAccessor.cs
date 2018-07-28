using System;
using System.Threading;

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

        public CardReader OpenCardReader()
        {
            var handle = LibMilfareApi.ftrMFOpenDevice();

            if (handle != IntPtr.Zero)
            {
                return new CardReader(handle);
            }

            throw new Exception("Cannot open device");
        }
    }

    enum CardType
    {
        Invalid = 0,
        OtherOrCreditCard = 4,
        Mifare1K = 8,
        Mifare4K = 18
    }

    public class CardReader : IDisposable
    {
        private IntPtr handle;
        private Timer cardDetectionTimer;


        public CardReader(IntPtr handle)
        {
            this.handle = handle;

            //this.cardDetectionTimer = new Timer(this.CardDetectionCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void StartCardDetection()
        {
            CardDetectionCallback(new object());
        }

        private void CardDetectionCallback(object state)
        {
        }

        public ulong GetCardSerialNumber()
        {
            var openSequence = LibMilfareApi.ftrMFStartSequence(handle);

            var cardTypeBytes = new byte[1];
            var serialNumberBytes = new byte[8];
            
            LibMilfareApi.ftrMFActivateIdle8bSN(this.handle, cardTypeBytes, serialNumberBytes);

            LibMilfareApi.ftrMFEndSequence(this.handle);

            var cardType = (CardType)cardTypeBytes[0];

            if (cardType == CardType.Invalid)
            {
                return 0;
            }

            var serialNumber = BitConverter.ToUInt64(serialNumberBytes, 0);

            return serialNumber;
        }

        public void Dispose()
        {
            LibMilfareApi.ftrMFCloseDevice(this.handle);
        }
    }
}