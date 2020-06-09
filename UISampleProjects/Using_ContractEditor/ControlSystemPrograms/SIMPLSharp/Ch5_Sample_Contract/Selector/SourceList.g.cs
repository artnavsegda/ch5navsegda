using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Selector
{
    public interface ISourceList
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> SelectedSourceIndex;

        /// <summary>
        /// The number of active sources in a list
        /// </summary>
        void ActiveSources(SourceListUShortInputSigDelegate callback);
        void SelectedSource(SourceListStringInputSigDelegate callback);
        void SelectedIconPath(SourceListStringInputSigDelegate callback);

        Ch5_Sample_Contract.Selector.ISource[] Source { get; }
    }

    public delegate void SourceListUShortInputSigDelegate(UShortInputSig uShortInputSig, ISourceList sourceList);
    public delegate void SourceListStringInputSigDelegate(StringInputSig stringInputSig, ISourceList sourceList);

    public class SourceList : ISourceList, IDisposable
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
            internal class Numerics
            {
                public const uint SelectedSourceIndex = 2;

                public const uint ActiveSources = 1;
            }
            internal class Strings
            {

                public const uint SelectedSource = 1;
                public const uint SelectedIconPath = 2;
            }
        }

        #endregion

        #region Construction and Initialization

        internal SourceList(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            Initialize(devices, controlJoinId);
        }

        internal SourceList(BasicTriListWithSmartObject device, uint controlJoinId)
            : this(new [] { device }, controlJoinId)
        {
        }

        private readonly IDictionary<uint, List<uint>> _sourceSmartObjectIdMappings = new Dictionary<uint, List<uint>> { { 32, new List<uint> { 33, 34, 35, 36, 37, 38, 39, 40 } } };

        private void Initialize(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            if (_devices == null)
            {
                ControlJoinId = controlJoinId; 
 
                _devices = new List<BasicTriListWithSmartObject>(); 
 
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics.SelectedSourceIndex, onSelectedSourceIndex);
                
                List<uint> sourceList = _sourceSmartObjectIdMappings[controlJoinId];
                Source = new Ch5_Sample_Contract.Selector.ISource[sourceList.Count];
                for (int index = 0; index < sourceList.Count; index++)
                {
                    Source[index] = new Ch5_Sample_Contract.Selector.Source(devices, sourceList[index]); 
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
            for (int index = 0; index < Source.Length; index++)
            {
                ((Ch5_Sample_Contract.Selector.Source)Source[index]).AddDevice(device);
            }
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.Instance.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
            for (int index = 0; index < Source.Length; index++)
            {
                ((Ch5_Sample_Contract.Selector.Source)Source[index]).RemoveDevice(device);
            }
        }

        #endregion

        #region CH5 Contract

        public Ch5_Sample_Contract.Selector.ISource[] Source { get; private set; }

        public event EventHandler<UIEventArgs> SelectedSourceIndex;
        private void onSelectedSourceIndex(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = SelectedSourceIndex;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void ActiveSources(SourceListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.ActiveSources], this);
            }
        }


        public void SelectedSource(SourceListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedSource], this);
            }
        }

        public void SelectedIconPath(SourceListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedIconPath], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "SourceList", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            for (int index = 0; index < Source.Length; index++)
            {
                ((Ch5_Sample_Contract.Selector.Source)Source[index]).Dispose();
            }

            SelectedSourceIndex = null;
        }

        #endregion

    }
}
