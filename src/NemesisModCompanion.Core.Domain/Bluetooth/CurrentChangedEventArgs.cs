using System;

namespace NemesisModCompanion.Core.Domain.Bluetooth
{
    public class CurrentChangedEventArgs
    {
        public int Milliamps { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}