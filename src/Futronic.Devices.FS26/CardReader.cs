using System;
using System.Threading;

namespace Futronic.Devices.FS26
{
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
            LibMifareApi.ftrMFStartSequence(handle);

            var cardTypeBytes = new byte[1];
            var serialNumberBytes = new byte[8];
            
            LibMifareApi.ftrMFActivateIdle8bSN(this.handle, cardTypeBytes, serialNumberBytes);

            LibMifareApi.ftrMFEndSequence(this.handle);

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
            LibMifareApi.ftrMFCloseDevice(this.handle);
        }
    }
}