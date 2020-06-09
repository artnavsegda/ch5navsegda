using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;

namespace CH5SampleSSP.Video
{
    /// <summary>
    /// the source type of video URL (e.g. "Network")
    /// </summary>
    public interface ICamera
    {
        object UserObject { get; set; }

        void TimePeriodOfImage(CameraUShortInputSigDelegate callback);
        void NameOfCamera(CameraStringInputSigDelegate callback);
        void VideoURLOfCamera(CameraStringInputSigDelegate callback);
        void ImageURLOfCamera(CameraStringInputSigDelegate callback);
        void SourceTypeOfCamera(CameraStringInputSigDelegate callback);
        void UserIdOfVideo(CameraStringInputSigDelegate callback);
        void PasswordOfVideo(CameraStringInputSigDelegate callback);
        void UserIdOfImage(CameraStringInputSigDelegate callback);
        void PasswordOfImage(CameraStringInputSigDelegate callback);

    }

    public delegate void CameraUShortInputSigDelegate(UShortInputSig uShortInputSig, ICamera camera);
    public delegate void CameraStringInputSigDelegate(StringInputSig stringInputSig, ICamera camera);

    /// <summary>
    /// Per video source
    /// </summary>
    internal class Camera : ICamera, IDisposable
    {
        #region Standard CH5 Component members

        private ComponentMediator ComponentMediator { get; set; }

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

                public const uint TimePeriodOfImage = 1;
            }
            internal class Strings
            {

                public const uint NameOfCamera = 1;
                public const uint VideoURLOfCamera = 2;
                public const uint ImageURLOfCamera = 3;
                public const uint SourceTypeOfCamera = 4;
                public const uint UserIdOfVideo = 5;
                public const uint PasswordOfVideo = 6;
                public const uint UserIdOfImage = 7;
                public const uint PasswordOfImage = 8;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Camera(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
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


        public void TimePeriodOfImage(CameraUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.TimePeriodOfImage], this);
            }
        }


        public void NameOfCamera(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.NameOfCamera], this);
            }
        }

        public void VideoURLOfCamera(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.VideoURLOfCamera], this);
            }
        }

        public void ImageURLOfCamera(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.ImageURLOfCamera], this);
            }
        }

        public void SourceTypeOfCamera(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SourceTypeOfCamera], this);
            }
        }

        public void UserIdOfVideo(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.UserIdOfVideo], this);
            }
        }

        public void PasswordOfVideo(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.PasswordOfVideo], this);
            }
        }

        public void UserIdOfImage(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.UserIdOfImage], this);
            }
        }

        public void PasswordOfImage(CameraStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.PasswordOfImage], this);
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

        }

        #endregion

    }
}
