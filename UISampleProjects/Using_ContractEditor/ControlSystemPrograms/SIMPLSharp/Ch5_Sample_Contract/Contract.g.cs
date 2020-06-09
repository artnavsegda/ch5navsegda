using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract
{
    /// <summary>
    /// An example of using compound CH5 contracts to produce one system contract
    /// </summary>
    public class Contract : IDisposable
    {
        #region Components

        public Ch5_Sample_Contract.Contact.IContactList ContactList { get { return (Ch5_Sample_Contract.Contact.IContactList)InternalContactList; } }
        private Ch5_Sample_Contract.Contact.ContactList InternalContactList { get; set; }

        public Ch5_Sample_Contract.Selector.ISourceList SourceList { get { return (Ch5_Sample_Contract.Selector.ISourceList)InternalSourceList; } }
        private Ch5_Sample_Contract.Selector.SourceList InternalSourceList { get; set; }

        public Ch5_Sample_Contract.Lighting.ILightingList LightingList { get { return (Ch5_Sample_Contract.Lighting.ILightingList)InternalLightingList; } }
        private Ch5_Sample_Contract.Lighting.LightingList InternalLightingList { get; set; }

        public Ch5_Sample_Contract.Video.ICameraList CameraList { get { return (Ch5_Sample_Contract.Video.ICameraList)InternalCameraList; } }
        private Ch5_Sample_Contract.Video.CameraList InternalCameraList { get; set; }

        #endregion

        #region Construction and Initialization

        public Contract()
            : this(new List<BasicTriListWithSmartObject>().ToArray())
        {
        }

        public Contract(BasicTriListWithSmartObject device)
            : this(new [] { device })
        {
        }

        public Contract(BasicTriListWithSmartObject[] devices)
        {
            if (devices == null)
                throw new ArgumentNullException("Devices is null");

            InternalContactList = new Ch5_Sample_Contract.Contact.ContactList(devices, 1);
            InternalSourceList = new Ch5_Sample_Contract.Selector.SourceList(devices, 32);
            InternalLightingList = new Ch5_Sample_Contract.Lighting.LightingList(devices, 41);
            InternalCameraList = new Ch5_Sample_Contract.Video.CameraList(devices, 50);
        }

        #endregion

        #region Standard Contract Members

        public object UserObject { get; set; }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            InternalContactList.AddDevice(device);
            InternalSourceList.AddDevice(device);
            InternalLightingList.AddDevice(device);
            InternalCameraList.AddDevice(device);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            InternalContactList.RemoveDevice(device);
            InternalSourceList.RemoveDevice(device);
            InternalLightingList.RemoveDevice(device);
            InternalCameraList.RemoveDevice(device);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            InternalContactList.Dispose();
            InternalSourceList.Dispose();
            InternalLightingList.Dispose();
            InternalCameraList.Dispose();
            ComponentMediator.Instance.Dispose(); 
        }

        #endregion

    }
}
