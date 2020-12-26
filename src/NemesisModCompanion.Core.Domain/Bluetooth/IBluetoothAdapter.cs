using System;
using System.Threading.Tasks;

namespace NemesisModCompanion.Core.Domain.Bluetooth
{
    public interface IBluetoothAdapter
    {
        event EventHandler<CurrentChangedEventArgs> FlywheelM1CurrentMilliampsChanged;

        event EventHandler<CurrentChangedEventArgs> FlywheelM2CurrentMilliampsChanged;

        event EventHandler<CurrentChangedEventArgs> BeltM1CurrentMilliampsChanged;

        bool IsAttached { get; }

        void StartMonitoring();

        Task ChangeHopperLockEnabled(bool value);

        Task<bool> GetHopperLockEnabled();

        Task<float> GetFlywheelTrimVariance();

        Task<int> GetBeltMaxSpeed();

        Task<int> GetBeltMediumSpeed();

        Task<int> GetBeltNormalSpeed();

        Task<byte> GetCurrentBeltSpeed();

        Task<byte> GetCurrentFlywheelSpeed();

        Task<int> GetFlywheelKidSpeed();

        Task<int> GetFlywheelNormalSpeed();

        Task<int> GetFlywheelLudicrousSpeed();

        Task<float> GetFlywheelM1TrimSpeed();

        Task<float> GetFlywheelM2TrimSpeed();
        Task AttachToDevice();
        Task ConnectAsync();

        Task ChangeTrimSpeeds(float m1TrimValue, float m2TrimValue);

        Task ChangeFlywheelKidSpeed(int value);

        Task ChangeFlywheelNormalSpeed(int value);

        Task ChangeFlywheelLudicrousSpeed(int value);

        Task ChangeFeedNormalSpeed(int value);

        Task ChangeFeedMediumSpeed(int value);

        Task ChangeFeedMaxSpeed(int value);

        Task ChangeBeltSpeed(byte value);

        Task ChangeFlywheelSpeed(byte value);

        Task ChangeFlywheelTrimVariance(float value);
    }
}