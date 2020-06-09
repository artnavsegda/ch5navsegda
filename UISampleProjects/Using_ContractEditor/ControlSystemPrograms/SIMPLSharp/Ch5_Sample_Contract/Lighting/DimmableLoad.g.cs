using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Lighting
{
    public interface IDimmableLoad
    {
        object UserObject { get; set; }

        /// <summary>
        /// Light On
        /// </summary>
        event EventHandler<UIEventArgs> On;
        /// <summary>
        /// Light Off
        /// </summary>
        event EventHandler<UIEventArgs> Off;
        event EventHandler<UIEventArgs> Level;

        /// <summary>
        /// selected for ramping
        /// </summary>
        void Selected(DimmableLoadBoolInputSigDelegate callback);
        /// <summary>
        /// light level
        /// </summary>
        void Level_Feedback(DimmableLoadUShortInputSigDelegate callback);
        /// <summary>
        /// Lighting load name
        /// </summary>
        void Name(DimmableLoadStringInputSigDelegate callback);

    }

    public delegate void DimmableLoadBoolInputSigDelegate(BoolInputSig boolInputSig, IDimmableLoad dimmableLoad);
    public delegate void DimmableLoadUShortInputSigDelegate(UShortInputSig uShortInputSig, IDimmableLoad dimmableLoad);
    public delegate void DimmableLoadStringInputSigDelegate(StringInputSig stringInputSig, IDimmableLoad dimmableLoad);

    public class DimmableLoad : IDimmableLoad, IDisposable
    {
        #region Standard CH5 Component members

        public object UserObject { get; set; }

        public uint ControlJoinId { get; private set; }

        private IList<BasicTriListWithSmartObject> _devices;
        public IList<BasicTriListWithSmartObject> Devices { get { return _devices; } }

        #endregion

        #region Joins

        private class Joins
        {
            internal class Booleans
            {
                public const uint On = 2;
                public const uint Off = 3;

                public const uint Selected = 1;
            }
            internal class Numerics
            {
                public const uint Level = 1;

                public const uint Level_Feedback = 1;
            }
            internal class Strings
            {

                public const uint Name = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal DimmableLoad(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            Initialize(devices, controlJoinId);
        }

        internal DimmableLoad(BasicTriListWithSmartObject device, uint controlJoinId)
            : this(new [] { device }, controlJoinId)
        {
        }

        private void Initialize(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            if (_devices == null)
            {
                ControlJoinId = controlJoinId; 
 
                _devices = new List<BasicTriListWithSmartObject>(); 
 
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.On, onOn);
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Off, onOff);
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics.Level, onLevel);
                
                ConfigureSmartObjectHandler(devices); 
            }
        }

        private void ConfigureSmartObjectHandler(BasicTriListWithSmartObject[] devices)
        {
            for (int index = 0; index < devices.Length; index++)
            {
                AddDevice(devices[index]);
            }
        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.Instance.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.Instance.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        #endregion

        #region CH5 Contract

        public event EventHandler<UIEventArgs> On;
        private void onOn(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = On;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> Off;
        private void onOff(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Off;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void Selected(DimmableLoadBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Selected], this);
            }
        }

        public event EventHandler<UIEventArgs> Level;
        private void onLevel(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Level;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void Level_Feedback(DimmableLoadUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Level_Feedback], this);
            }
        }


        public void Name(DimmableLoadStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Name], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "DimmableLoad", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            On = null;
            Off = null;
            Level = null;
        }

        #endregion

    }
}
