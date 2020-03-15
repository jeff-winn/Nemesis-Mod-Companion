using System;

namespace NemesisModCompanion.Core.Domain.Bluetooth
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Value { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}