using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Video
{
    public interface ICamera
    {
        object UserObject { get; set; }

        /// <summary>
        /// Select this camera
        /// </summary>
        event EventHandler<UIEventArgs> Select;

        /// <summary>
        /// Camera is current selection
        /// </summary>
        void Selected(CameraBoolInputSigDelegate callback);
        void ImagePeriod(CameraUShortInputSigDelegate callback);
        void VideoURL(CameraStringInputSigDelegate callback);
        void ImageURL(CameraStringInputSigDelegate callback);
        void SourceType(CameraStringInputSigDelegate callback);
        void Name(CameraStringInputSigDelegate callback);
        void VideoUserId(CameraStringInputSigDelegate callback);
        void VideoPassword(CameraStringInputSigDelegate callback);
        /// <summary>
        /// Image user ID
        /// </summary>
        void ImageUserId(CameraStringInputSigDelegate callback);
        void ImagePassword(CameraStringInputSigDelegate callback);

    }

    public delegate void CameraBoolInputSigDelegate(BoolInputSig boolInputSig, ICamera camera);
    public delegate void CameraUShortInputSigDelegate(UShortInputSig uShortInputSig, ICamera camera);
    public delegate void CameraStringInputSigDelegate(StringInputSig stringInputSig, ICamera camera);

    /// <summary>
    /// PTZ Camera
    /// </summary>
    public class Camera : ICamera, IDisposable
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
            internal class Numerics
            {

                public const uint ImagePeriod = 1;
            }
            internal class Strings
            {

                public const uint VideoURL = 1;
                public const uint ImageURL = 2;
                public const uint SourceType = 3;
                public const uint Name = 4;
                public const uint VideoUserId = 5;
                public const uint VideoPassword = 6;
                public const uint ImageUserId = 7;
                public const uint ImagePassword = 8;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Camera(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            Initialize(devices, controlJoinId);
        }

        internal Camera(BasicTriListWithSmartObject device, uint controlJoinId)
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


        public void Selected(CameraBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Selected], this);
            }
        }


        public void ImagePeriod(CameraUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.ImagePeriod], this);
            }
        }


        public void VideoURL(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.VideoURL], this);
            }
        }

        public void ImageURL(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.ImageURL], this);
            }
        }

        public void SourceType(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SourceType], this);
            }
        }

        public void Name(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Name], this);
            }
        }

        public void VideoUserId(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.VideoUserId], this);
            }
        }

        public void VideoPassword(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.VideoPassword], this);
            }
        }

        public void ImageUserId(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.ImageUserId], this);
            }
        }

        public void ImagePassword(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.ImagePassword], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "Camera", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
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
