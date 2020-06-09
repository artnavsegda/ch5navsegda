using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Contact
{
    /// <summary>
    /// The number of active contacts
    /// </summary>
    public interface IContactList
    {
        object UserObject { get; set; }

        void NumberOfContacts(ContactListUShortInputSigDelegate callback);
        void SelectedName(ContactListStringInputSigDelegate callback);
        void SelectedCompany(ContactListStringInputSigDelegate callback);
        void SelectedTitle(ContactListStringInputSigDelegate callback);
        void SelectedEmail(ContactListStringInputSigDelegate callback);
        void SelectedWorkPhone(ContactListStringInputSigDelegate callback);
        void SelectedExtension(ContactListStringInputSigDelegate callback);
        void SelectedMobilePhone(ContactListStringInputSigDelegate callback);
        void SelectedHomePhone(ContactListStringInputSigDelegate callback);
        void SelectedImage(ContactListStringInputSigDelegate callback);
        void SelectedNickName(ContactListStringInputSigDelegate callback);

        Ch5_Sample_Contract.Contact.IContact[] Contact { get; }
    }

    public delegate void ContactListUShortInputSigDelegate(UShortInputSig uShortInputSig, IContactList contactList);
    public delegate void ContactListStringInputSigDelegate(StringInputSig stringInputSig, IContactList contactList);

    internal class ContactList : IContactList, IDisposable
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
            internal static class Numerics
            {

                public const uint NumberOfContacts = 1;
            }
            internal static class Strings
            {

                public const uint SelectedName = 1;
                public const uint SelectedCompany = 2;
                public const uint SelectedTitle = 3;
                public const uint SelectedEmail = 4;
                public const uint SelectedWorkPhone = 5;
                public const uint SelectedExtension = 6;
                public const uint SelectedMobilePhone = 7;
                public const uint SelectedHomePhone = 8;
                public const uint SelectedImage = 9;
                public const uint SelectedNickName = 10;
            }
        }

        #endregion

        #region Construction and Initialization

        internal ContactList(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private static readonly IDictionary<uint, List<uint>> ContactSmartObjectIdMappings = new Dictionary<uint, List<uint>> {
            { 1, new List<uint> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 } }};

        internal static void ClearDictionaries()
        {
            ContactSmartObjectIdMappings.Clear();
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            List<uint> contactList = ContactSmartObjectIdMappings[controlJoinId];
            Contact = new Ch5_Sample_Contract.Contact.IContact[contactList.Count];
            for (int index = 0; index < contactList.Count; index++)
            {
                Contact[index] = new Ch5_Sample_Contract.Contact.Contact(ComponentMediator, contactList[index]); 
            }

        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
            for (int index = 0; index < Contact.Length; index++)
            {
                ((Ch5_Sample_Contract.Contact.Contact)Contact[index]).AddDevice(device);
            }
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
            for (int index = 0; index < Contact.Length; index++)
            {
                ((Ch5_Sample_Contract.Contact.Contact)Contact[index]).RemoveDevice(device);
            }
        }

        #endregion

        #region CH5 Contract

        public Ch5_Sample_Contract.Contact.IContact[] Contact { get; private set; }


        public void NumberOfContacts(ContactListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.NumberOfContacts], this);
            }
        }


        public void SelectedName(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedName], this);
            }
        }

        public void SelectedCompany(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedCompany], this);
            }
        }

        public void SelectedTitle(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedTitle], this);
            }
        }

        public void SelectedEmail(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedEmail], this);
            }
        }

        public void SelectedWorkPhone(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedWorkPhone], this);
            }
        }

        public void SelectedExtension(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedExtension], this);
            }
        }

        public void SelectedMobilePhone(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedMobilePhone], this);
            }
        }

        public void SelectedHomePhone(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedHomePhone], this);
            }
        }

        public void SelectedImage(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedImage], this);
            }
        }

        public void SelectedNickName(ContactListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedNickName], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "ContactList", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            for (int index = 0; index < Contact.Length; index++)
            {
                ((Ch5_Sample_Contract.Contact.Contact)Contact[index]).Dispose();
            }

        }

        #endregion

    }
}
