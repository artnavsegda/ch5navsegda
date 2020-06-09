using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Video
{
    public interface ICameraList
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> CameraStatate;
        event EventHandler<UIEventArgs> CameraErrorCode;
        event EventHandler<UIEventArgs> _CameraVideoRetryCount;
        event EventHandler<UIEventArgs> SelectedCameraIndex;
        event EventHandler<UIEventArgs> PlayingCameraIndex;
        event EventHandler<UIEventArgs> PlayingCameraStillImageStatus;
        event EventHandler<UIEventArgs> CameraErrorMessage;
        event EventHandler<UIEventArgs> CameraVideoResolution;
        event EventHandler<UIEventArgs> PlayingCameraSourceType;
        event EventHandler<UIEventArgs> PlayingCameraVideoURL;
        event EventHandler<UIEventArgs> PlayingCameraStillImageLastUpdateTime;
        event EventHandler<UIEventArgs> PlayingCameraImageURL;

        void CameraPlaying(CameraListBoolInputSigDelegate callback);
        void SelectedCameraState(CameraListUShortInputSigDelegate callback);
        void SelectedCameraErrorCode(CameraListUShortInputSigDelegate callback);
        void SelectedCameraVideoRetryCount(CameraListUShortInputSigDelegate callback);
        void SelectedCameraNumber(CameraListUShortInputSigDelegate callback);
        void PlayingCameraNumber(CameraListUShortInputSigDelegate callback);
        void SelectedCameraStillImageStatus(CameraListUShortInputSigDelegate callback);
        void ActiveCameras(CameraListUShortInputSigDelegate callback);
        void SelectedCameraErrorMessage(CameraListStringInputSigDelegate callback);
        void SelectedCameraVideoResolution(CameraListStringInputSigDelegate callback);
        void SelectedCameraSourceType(CameraListStringInputSigDelegate callback);
        void SelectedCameraVideoURL(CameraListStringInputSigDelegate callback);
        void SelectedCameraStillImageLastUpdateTime(CameraListStringInputSigDelegate callback);
        void PlayingCameraImageURL_Fbq(CameraListStringInputSigDelegate callback);
        void SelectedCameraName(CameraListStringInputSigDelegate callback);
        void SelectedVideoUserId(CameraListStringInputSigDelegate callback);

        Ch5_Sample_Contract.Video.ICamera[] Camera { get; }
    }

    public delegate void CameraListBoolInputSigDelegate(BoolInputSig boolInputSig, ICameraList cameraList);
    public delegate void CameraListUShortInputSigDelegate(UShortInputSig uShortInputSig, ICameraList cameraList);
    public delegate void CameraListStringInputSigDelegate(StringInputSig stringInputSig, ICameraList cameraList);

    /// <summary>
    /// A container for an array of camera components
    /// </summary>
    public class CameraList : ICameraList, IDisposable
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

                public const uint CameraPlaying = 1;
            }
            internal class Numerics
            {
                public const uint CameraStatate = 1;
                public const uint CameraErrorCode = 2;
                public const uint _CameraVideoRetryCount = 3;
                public const uint SelectedCameraIndex = 4;
                public const uint PlayingCameraIndex = 5;
                public const uint PlayingCameraStillImageStatus = 6;

                public const uint SelectedCameraState = 1;
                public const uint SelectedCameraErrorCode = 2;
                public const uint SelectedCameraVideoRetryCount = 3;
                public const uint SelectedCameraNumber = 4;
                public const uint PlayingCameraNumber = 5;
                public const uint SelectedCameraStillImageStatus = 6;
                public const uint ActiveCameras = 7;
            }
            internal class Strings
            {
                public const uint CameraErrorMessage = 1;
                public const uint CameraVideoResolution = 2;
                public const uint PlayingCameraSourceType = 3;
                public const uint PlayingCameraVideoURL = 4;
                public const uint PlayingCameraStillImageLastUpdateTime = 5;
                public const uint PlayingCameraImageURL = 6;

                public const uint SelectedCameraErrorMessage = 1;
                public const uint SelectedCameraVideoResolution = 2;
                public const uint SelectedCameraSourceType = 3;
                public const uint SelectedCameraVideoURL = 4;
                public const uint SelectedCameraStillImageLastUpdateTime = 5;
                public const uint PlayingCameraImageURL_Fbq = 6;
                public const uint SelectedCameraName = 7;
                public const uint SelectedVideoUserId = 8;
            }
        }

        #endregion

        #region Construction and Initialization

        internal CameraList(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            Initialize(devices, controlJoinId);
        }

        internal CameraList(BasicTriListWithSmartObject device, uint controlJoinId)
            : this(new [] { device }, controlJoinId)
        {
        }

        private readonly IDictionary<uint, List<uint>> _cameraSmartObjectIdMappings = new Dictionary<uint, List<uint>> { { 50, new List<uint> { 51, 52, 53, 54, 55, 56, 57, 58, 59, 60 } } };

        private void Initialize(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            if (_devices == null)
            {
                ControlJoinId = controlJoinId; 
 
                _devices = new List<BasicTriListWithSmartObject>(); 
 
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics.CameraStatate, onCameraStatate);
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics.CameraErrorCode, onCameraErrorCode);
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics._CameraVideoRetryCount, on_CameraVideoRetryCount);
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics.SelectedCameraIndex, onSelectedCameraIndex);
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics.PlayingCameraIndex, onPlayingCameraIndex);
                ComponentMediator.Instance.ConfigureNumericEvent(controlJoinId, Joins.Numerics.PlayingCameraStillImageStatus, onPlayingCameraStillImageStatus);
                ComponentMediator.Instance.ConfigureStringEvent(controlJoinId, Joins.Strings.CameraErrorMessage, onCameraErrorMessage);
                ComponentMediator.Instance.ConfigureStringEvent(controlJoinId, Joins.Strings.CameraVideoResolution, onCameraVideoResolution);
                ComponentMediator.Instance.ConfigureStringEvent(controlJoinId, Joins.Strings.PlayingCameraSourceType, onPlayingCameraSourceType);
                ComponentMediator.Instance.ConfigureStringEvent(controlJoinId, Joins.Strings.PlayingCameraVideoURL, onPlayingCameraVideoURL);
                ComponentMediator.Instance.ConfigureStringEvent(controlJoinId, Joins.Strings.PlayingCameraStillImageLastUpdateTime, onPlayingCameraStillImageLastUpdateTime);
                ComponentMediator.Instance.ConfigureStringEvent(controlJoinId, Joins.Strings.PlayingCameraImageURL, onPlayingCameraImageURL);
                
                List<uint> cameraList = _cameraSmartObjectIdMappings[controlJoinId];
                Camera = new Ch5_Sample_Contract.Video.ICamera[cameraList.Count];
                for (int index = 0; index < cameraList.Count; index++)
                {
                    Camera[index] = new Ch5_Sample_Contract.Video.Camera(devices, cameraList[index]); 
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
            for (int index = 0; index < Camera.Length; index++)
            {
                ((Ch5_Sample_Contract.Video.Camera)Camera[index]).AddDevice(device);
            }
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.Instance.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
            for (int index = 0; index < Camera.Length; index++)
            {
                ((Ch5_Sample_Contract.Video.Camera)Camera[index]).RemoveDevice(device);
            }
        }

        #endregion

        #region CH5 Contract

        public Ch5_Sample_Contract.Video.ICamera[] Camera { get; private set; }


        public void CameraPlaying(CameraListBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.CameraPlaying], this);
            }
        }

        public event EventHandler<UIEventArgs> CameraStatate;
        private void onCameraStatate(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = CameraStatate;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> CameraErrorCode;
        private void onCameraErrorCode(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = CameraErrorCode;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> _CameraVideoRetryCount;
        private void on_CameraVideoRetryCount(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = _CameraVideoRetryCount;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> SelectedCameraIndex;
        private void onSelectedCameraIndex(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = SelectedCameraIndex;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> PlayingCameraIndex;
        private void onPlayingCameraIndex(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = PlayingCameraIndex;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> PlayingCameraStillImageStatus;
        private void onPlayingCameraStillImageStatus(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = PlayingCameraStillImageStatus;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void SelectedCameraState(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.SelectedCameraState], this);
            }
        }

        public void SelectedCameraErrorCode(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.SelectedCameraErrorCode], this);
            }
        }

        public void SelectedCameraVideoRetryCount(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.SelectedCameraVideoRetryCount], this);
            }
        }

        public void SelectedCameraNumber(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.SelectedCameraNumber], this);
            }
        }

        public void PlayingCameraNumber(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.PlayingCameraNumber], this);
            }
        }

        public void SelectedCameraStillImageStatus(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.SelectedCameraStillImageStatus], this);
            }
        }

        public void ActiveCameras(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.ActiveCameras], this);
            }
        }

        public event EventHandler<UIEventArgs> CameraErrorMessage;
        private void onCameraErrorMessage(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = CameraErrorMessage;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> CameraVideoResolution;
        private void onCameraVideoResolution(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = CameraVideoResolution;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> PlayingCameraSourceType;
        private void onPlayingCameraSourceType(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = PlayingCameraSourceType;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> PlayingCameraVideoURL;
        private void onPlayingCameraVideoURL(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = PlayingCameraVideoURL;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> PlayingCameraStillImageLastUpdateTime;
        private void onPlayingCameraStillImageLastUpdateTime(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = PlayingCameraStillImageLastUpdateTime;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> PlayingCameraImageURL;
        private void onPlayingCameraImageURL(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = PlayingCameraImageURL;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void SelectedCameraErrorMessage(CameraListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedCameraErrorMessage], this);
            }
        }

        public void SelectedCameraVideoResolution(CameraListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedCameraVideoResolution], this);
            }
        }

        public void SelectedCameraSourceType(CameraListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedCameraSourceType], this);
            }
        }

        public void SelectedCameraVideoURL(CameraListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedCameraVideoURL], this);
            }
        }

        public void SelectedCameraStillImageLastUpdateTime(CameraListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedCameraStillImageLastUpdateTime], this);
            }
        }

        public void PlayingCameraImageURL_Fbq(CameraListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.PlayingCameraImageURL_Fbq], this);
            }
        }

        public void SelectedCameraName(CameraListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedCameraName], this);
            }
        }

        public void SelectedVideoUserId(CameraListStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.SelectedVideoUserId], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "CameraList", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            for (int index = 0; index < Camera.Length; index++)
            {
                ((Ch5_Sample_Contract.Video.Camera)Camera[index]).Dispose();
            }

            CameraStatate = null;
            CameraErrorCode = null;
            _CameraVideoRetryCount = null;
            SelectedCameraIndex = null;
            PlayingCameraIndex = null;
            PlayingCameraStillImageStatus = null;
            CameraErrorMessage = null;
            CameraVideoResolution = null;
            PlayingCameraSourceType = null;
            PlayingCameraVideoURL = null;
            PlayingCameraStillImageLastUpdateTime = null;
            PlayingCameraImageURL = null;
        }

        #endregion

    }
}
