using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;

namespace CH5SampleSSP.Lighting
{
    public interface ILightingList
    {
        object UserObject { get; set; }

        /// <summary>
        /// turn all lights on
        /// </summary>
        event EventHandler<UIEventArgs> Scene_0;
        /// <summary>
        /// turn all lights off
        /// </summary>
        event EventHandler<UIEventArgs> Scene_1;
        /// <summary>
        /// set lights to High setting
        /// </summary>
        event EventHandler<UIEventArgs> Scene_2;
        /// <summary>
        /// set lights to Medium setting
        /// </summary>
        event EventHandler<UIEventArgs> Scene_3;
        /// <summary>
        /// set lights to Low setting
        /// </summary>
        event EventHandler<UIEventArgs> Scene_4;
        /// <summary>
        /// select this dimmer
        /// </summary>
        event EventHandler<UIEventArgs> SelectedDimmerIndex;

        /// <summary>
        /// number of active dimmers
        /// </summary>
        void ActiveDimmers(LightingListUShortInputSigDelegate callback);

        IDimmableLoad[] DimmableLoad { get; }
    }

    public delegate void LightingListBoolInputSigDelegate(BoolInputSig boolInputSig, ILightingList lightingList);
    public delegate void LightingListUShortInputSigDelegate(UShortInputSig uShortInputSig, ILightingList lightingList);

    /// <summary>
    /// an array of dimmers
    /// </summary>
    public class LightingList : ILightingList, IDisposable
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
                public const uint Scene_0 = 1;
                public const uint Scene_1 = 2;
                public const uint Scene_2 = 3;
                public const uint Scene_3 = 4;
                public const uint Scene_4 = 5;

            }
            internal class Numerics
            {
                public const uint SelectedDimmerIndex = 2;

                public const uint ActiveDimmers = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal LightingList(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            Initialize(devices, controlJoinId);
        }

        internal LightingList(BasicTriListWithSmartObject device, uint controlJoinId)
            : this(new [] { device }, controlJoinId)
        {
        }

        private readonly IDictionary<uint, List<uint>> _dimmableLoadSmartObjectIdMappings = new Dictionary<uint, List<uint>> { { 41, new List<uint> { 42, 43, 44, 45, 46, 47, 48, 49 } } };

        private void Initialize(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            if (_devices == null)
            {
                ControlJoinId = controlJoinId; 
 
                _devices = new List<BasicTriListWithSmartObject>(); 
 
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Scene_0, onScene_0);
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Scene_1, onScene_1);
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Scene_2, onScene_2);
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Scene_3, onScene_3);
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Scene_4, onScene_4);
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics.SelectedDimmerIndex, onSelectedDimmerIndex);
                
                List<uint> dimmableLoadList = _dimmableLoadSmartObjectIdMappings[controlJoinId];
                DimmableLoad = new IDimmableLoad[dimmableLoadList.Count];
                for (int index = 0; index < dimmableLoadList.Count; index++)
                {
                    DimmableLoad[index] = new DimmableLoad(devices, dimmableLoadList[index]); 
                }
                
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
            for (int index = 0; index < DimmableLoad.Length; index++)
            {
                ((DimmableLoad)DimmableLoad[index]).AddDevice(device);
            }
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.Instance.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
            for (int index = 0; index < DimmableLoad.Length; index++)
            {
                ((DimmableLoad)DimmableLoad[index]).RemoveDevice(device);
            }
        }

        #endregion

        #region CH5 Contract

        public IDimmableLoad[] DimmableLoad { get; private set; }

        public event EventHandler<UIEventArgs> Scene_0;
        private void onScene_0(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Scene_0;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> Scene_1;
        private void onScene_1(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Scene_1;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> Scene_2;
        private void onScene_2(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Scene_2;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> Scene_3;
        private void onScene_3(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Scene_3;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> Scene_4;
        private void onScene_4(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Scene_4;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public event EventHandler<UIEventArgs> SelectedDimmerIndex;
        private void onSelectedDimmerIndex(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = SelectedDimmerIndex;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void ActiveDimmers(LightingListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.ActiveDimmers], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "LightingList", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            for (int index = 0; index < DimmableLoad.Length; index++)
            {
                ((DimmableLoad)DimmableLoad[index]).Dispose();
            }

            Scene_0 = null;
            Scene_1 = null;
            Scene_2 = null;
            Scene_3 = null;
            Scene_4 = null;
            SelectedDimmerIndex = null;
        }

        #endregion

    }
}
