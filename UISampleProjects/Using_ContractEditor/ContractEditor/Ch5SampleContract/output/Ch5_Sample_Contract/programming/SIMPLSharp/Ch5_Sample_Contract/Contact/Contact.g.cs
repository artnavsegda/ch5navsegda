using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Contact
{
    /// <summary>
    /// Select this contact
    /// </summary>
    /// <summary>
    /// Current contact
    /// </summary>
    /// <summary>
    /// Name of the contact
    /// </summary>
    /// <summary>
    /// Name of contact's company
    /// </summary>
    /// <summary>
    /// Title of the contact
    /// </summary>
    /// <summary>
    /// Email address of the contact
    /// </summary>
    /// <summary>
    /// Work phone number of the contact
    /// </summary>
    /// <summary>
    /// Extension of the work number
    /// </summary>
    /// <summary>
    /// Mobile number of contact
    /// </summary>
    /// <summary>
    /// Home number of the contact
    /// </summary>
    /// <summary>
    /// Image of contact
    /// </summary>
    /// <summary>
    /// Display name of contact
    /// </summary>
    public interface IContact
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> SetContactSelected;

        void ContactIsSelected(ContactBoolInputSigDelegate callback);
        void FullName(ContactStringInputSigDelegate callback);
        void Company(ContactStringInputSigDelegate callback);
        void Title(ContactStringInputSigDelegate callback);
        void Email(ContactStringInputSigDelegate callback);
        void WorkNumber(ContactStringInputSigDelegate callback);
        void WorkNumberExtension(ContactStringInputSigDelegate callback);
        void MobileNumber(ContactStringInputSigDelegate callback);
        void HomeNumber(ContactStringInputSigDelegate callback);
        void Image(ContactStringInputSigDelegate callback);
        void Nickname(ContactStringInputSigDelegate callback);

    }

    public delegate void ContactBoolInputSigDelegate(BoolInputSig boolInputSig, IContact contact);
    public delegate void ContactStringInputSigDelegate(StringInputSig stringInputSig, IContact contact);

    /// <summary>
    /// Information about an individual
    /// </summary>
    public class Contact : IContact, IDisposable
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
                public const uint SetContactSelected = 1;

                public const uint ContactIsSelected = 1;
            }
            internal class Strings
            {

                public const uint FullName = 1;
                public const uint Company = 2;
                public const uint Title = 3;
                public const uint Email = 4;
                public const uint WorkNumber = 5;
                public const uint WorkNumberExtension = 6;
                public const uint MobileNumber = 7;
                public const uint HomeNumber = 8;
                public const uint Image = 9;
                public const uint Nickname = 10;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Contact(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            Initialize(devices, controlJoinId);
        }

        internal Contact(BasicTriListWithSmartObject device, uint controlJoinId)
            : this(new [] { device }, controlJoinId)
        {
        }

        private void Initialize(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            if (_devices == null)
            {
                ControlJoinId = controlJoinId; 
 
                _devices = new List<BasicTriListWithSmartObject>(); 
 
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.SetContactSelected, onSetContactSelected);
                
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

        public event EventHandler<UIEventArgs> SetContactSelected;
        private void onSetContactSelected(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = SetContactSelected;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void ContactIsSelected(ContactBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.ContactIsSelected], this);
            }
        }


        public void FullName(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.FullName], this);
            }
        }

        public void Company(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Company], this);
            }
        }

        public void Title(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Title], this);
            }
        }

        public void Email(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Email], this);
            }
        }

        public void WorkNumber(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.WorkNumber], this);
            }
        }

        public void WorkNumberExtension(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.WorkNumberExtension], this);
            }
        }

        public void MobileNumber(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.MobileNumber], this);
            }
        }

        public void HomeNumber(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.HomeNumber], this);
            }
        }

        public void Image(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Image], this);
            }
        }

        public void Nickname(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Nickname], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "Contact", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            SetContactSelected = null;
        }

        #endregion

    }
}
