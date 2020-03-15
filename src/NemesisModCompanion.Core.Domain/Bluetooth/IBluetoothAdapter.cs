using System;

namespace NemesisModCompanion.Core.Domain.Bluetooth
{
    public interface IBluetoothAdapter
    {
        event EventHandler<ValueChangedEventArgs<int>> FlywheelM1CurrentMilliampsChanged;

        event EventHandler<ValueChangedEventArgs<int>> FlywheelM2CurrentMilliampsChanged;

        event EventHandler<ValueChangedEventArgs<int>> BeltM1CurrentMilliampsChanged;
    }
}