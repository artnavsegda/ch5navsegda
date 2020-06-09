using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Contact
{
    public interface IContact
    {
        object UserObject { get; set; }

        /// <summary>
        /// Select this contact
        /// </summary>
        event EventHandler<UIEventArgs> Select;

        /// <summary>
        /// Current contact
        /// </summary>
        void Selected(ContactBoolInputSigDelegate callback);
        /// <summary>
        /// Name of the contact
        /// </summary>
        void Full_Name(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Name of contact's company
        /// </summary>
        void Company(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Title of the contact
        /// </summary>
        void Title(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Email address of the contact
        /// </summary>
        void Email(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Work phone number of the contact
        /// </summary>
        void Work_Number(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Extension of the work number
        /// </summary>
        void Work_Number_Extension(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Mobile number of contact
        /// </summary>
        void Mobile_Number(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Home number of the contact
        /// </summary>
        void Home_Number(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Image of contact
        /// </summary>
        void Image(ContactStringInputSigDelegate callback);
        /// <summary>
        /// Display name of contact
        /// </summary>
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
                public const uint Select = 1;

                public const uint Selected = 1;
            }
            internal class Strings
            {

                public const uint Full_Name = 1;
                public const uint Company = 2;
                public const uint Title = 3;
                public const uint Email = 4;
                public const uint Work_Number = 5;
                public const uint Work_Number_Extension = 6;
                public const uint Mobile_Number = 7;
                public const uint Home_Number = 8;
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
 
                ComponentMediator.Instance.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Select, onSelect);
                
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

        public event EventHandler<UIEventArgs> Select;
        private void onSelect(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Select;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void Selected(ContactBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Selected], this);
            }
        }


        public void Full_Name(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Full_Name], this);
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

        public void Work_Number(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Work_Number], this);
            }
        }

        public void Work_Number_Extension(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Work_Number_Extension], this);
            }
        }

        public void Mobile_Number(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Mobile_Number], this);
            }
        }

        public void Home_Number(ContactStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Home_Number], this);
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

            Select = null;
        }

        #endregion

    }
}
