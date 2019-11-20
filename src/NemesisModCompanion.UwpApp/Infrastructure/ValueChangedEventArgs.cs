using System;

namespace NemesisModCompanion.UwpApp.Infrastructure
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Value { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}