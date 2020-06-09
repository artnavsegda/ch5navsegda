using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Lighting
{
    /// <summary>
    /// Light On
    /// </summary>
    /// <summary>
    /// Light Off
    /// </summary>
    /// <summary>
    /// request change to new level
    /// </summary>
    /// <summary>
    /// light level
    /// </summary>
    /// <summary>
    /// Lighting load name
    /// </summary>
    public interface IDimmableLight
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> TurnLightOn;
        event EventHandler<UIEventArgs> TurnLightOff;
        event EventHandler<UIEventArgs> SetLightLevel;

        void LightIsAtLevel(DimmableLightUShortInputSigDelegate callback);
        void NameOfLight(DimmableLightStringInputSigDelegate callback);

    }

    public delegate void DimmableLightBoolInputSigDelegate(BoolInputSig boolInputSig, IDimmableLight dimmableLight);
    public delegate void DimmableLightUShortInputSigDelegate(UShortInputSig uShortInputSig, IDimmableLight dimmableLight);
    public delegate void DimmableLightStringInputSigDelegate(StringInputSig stringInputSig, IDimmableLight dimmableLight);

    internal class DimmableLight : IDimmableLight, IDisposable
    {
        #region Standard CH5 Component members

        private ComponentMediator ComponentMediator { get; set; }

        public object UserObject { get; set; }

        public uint ControlJoinId { get; private set; }

        private IList<BasicTriListWithSmartObject> _devices;
        public IList<BasicTriListWithSmartObject> Devices { get { return _devices; } }

        #endregion

        #region Joins

        private static class Joins
        {
            internal static class Booleans
            {
                public const uint TurnLightOn = 1;
                public const uint TurnLightOff = 2;

            }
            internal static class Numerics
            {
                public const uint SetLightLevel = 1;

                public const uint LightIsAtLevel = 1;
            }
            internal static class Strings
            {

                public const uint NameOfLight = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal DimmableLight(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.TurnLightOn, onTurnLightOn);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.TurnLightOff, onTurnLightOff);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.SetLightLevel, onSetLightLevel);

        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        #endregion

        #region CH5 Contract

        public event EventHandler<UIEventArgs> TurnLightOn;
        private void onTurnLightOn(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = TurnLightOn;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> TurnLightOff;
        private void onTurnLightOff(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = TurnLightOff;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public event EventHandler<UIEventArgs> SetLightLevel;
        private void onSetLightLevel(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = SetLightLevel;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void LightIsAtLevel(DimmableLightUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.LightIsAtLevel], this);
            }
        }


        public void NameOfLight(DimmableLightStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.NameOfLight], this);
            }
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return (int)ControlJoinId;
        }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "DimmableLight", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            TurnLightOn = null;
            TurnLightOff = null;
            SetLightLevel = null;
        }

        #endregion

    }
}
