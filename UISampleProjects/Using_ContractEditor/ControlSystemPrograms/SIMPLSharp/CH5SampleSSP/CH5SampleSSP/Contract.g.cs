using System;
using System.Collections.Generic;
using CH5SampleSSP.Contact;
using CH5SampleSSP.Lighting;
using CH5SampleSSP.Selector;
using CH5SampleSSP.Video;
using Crestron.SimplSharpPro.DeviceSupport;

namespace CH5SampleSSP
{
    /// <summary>
    /// Common Interface for Root Contracts.
    /// </summary>
    public interface IContract
    {
        object UserObject { get; set; }
        void AddDevice(BasicTriListWithSmartObject device);
        void RemoveDevice(BasicTriListWithSmartObject device);
    }

    /// <summary>
    /// An example of using compound CH5 contracts to produce one system contract
    /// </summary>
    public class Contract : IContract, IDisposable
    {
        #region Components

        private ComponentMediator ComponentMediator { get; set; }

        public IContactList ContactList { get { return (IContactList)InternalContactList; } }
        private ContactList InternalContactList { get; set; }

        public ISourceList SourceList { get { return (ISourceList)InternalSourceList; } }
        private SourceList InternalSourceList { get; set; }

        public IRoom Room { get { return (IRoom)InternalRoom; } }
        private Room InternalRoom { get; set; }

        public ICameraList CameraList { get { return (ICameraList)InternalCameraList; } }
        private CameraList InternalCameraList { get; set; }

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

            ComponentMediator = new ComponentMediator();
            InternalContactList = new ContactList(ComponentMediator, 1);
            InternalSourceList = new SourceList(ComponentMediator, 32);
            InternalRoom = new Room(ComponentMediator, 41);
            InternalCameraList = new CameraList(ComponentMediator, 82);

            for (int index = 0; index < devices.Length; index++)
            {
                AddDevice(devices[index]);
            }
        }

        #endregion

        #region Standard Contract Members

        public object UserObject { get; set; }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            InternalContactList.AddDevice(device);
            InternalSourceList.AddDevice(device);
            InternalRoom.AddDevice(device);
            InternalCameraList.AddDevice(device);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            InternalContactList.RemoveDevice(device);
            InternalSourceList.RemoveDevice(device);
            InternalRoom.RemoveDevice(device);
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
            InternalRoom.Dispose();
            InternalCameraList.Dispose();
            ComponentMediator.Dispose(); 
        }

        #endregion

    }
}
