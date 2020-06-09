using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;

namespace CH5SampleSSP.Video
{
    /// <summary>
    /// request IndexOfSelectedCamera to change
    /// </summary>
    /// <summary>
    /// 0-no src, 1 - stopped, 2 - playing, 3 - stop req, 4/5/6 – play req, 7 - error
    /// </summary>
    /// <summary>
    /// 0 - no error, transient issues > 0, setup and system issues < 0
    /// </summary>
    /// <summary>
    /// number of times video engin has tried to render video
    /// </summary>
    /// <summary>
    /// 0 not shown, 1 - currently shown, 2- in error
    /// </summary>
    /// <summary>
    /// diagnostic information on errors, not intended for end user to see
    /// </summary>
    /// <summary>
    /// Video stream resolution
    /// </summary>
    /// <summary>
    /// time in RFC 3339 format
    /// </summary>
    /// <summary>
    /// the selected camera to be displayed on touchscreen
    /// </summary>
    public interface ICameraList
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> SetSelectedCameraIndex;
        event EventHandler<UIEventArgs> StateOfSelectedCamera;
        event EventHandler<UIEventArgs> ErrorCodeOfSelectedCamera;
        event EventHandler<UIEventArgs> RetryCountOfSelectedCamera;
        event EventHandler<UIEventArgs> StatusOfSelectedCameraImage;
        event EventHandler<UIEventArgs> ErrorMessageOfSelectedCamera;
        event EventHandler<UIEventArgs> ResolutionOfSelectedCamera;
        event EventHandler<UIEventArgs> LastUpdateTimeOfSelectedCameraImage;
        event EventHandler<UIEventArgs> URLOfVideo;
        event EventHandler<UIEventArgs> SourceTypeOfSelectedCamara;
        event EventHandler<UIEventArgs> ImageURLOfSelectedCamera;
        event EventHandler<UIEventArgs> VideoURLOfSelectedCamera;

        void NumberOfCameras(CameraListUShortInputSigDelegate callback);
        void IndexOfSelectedCamera(CameraListUShortInputSigDelegate callback);

        ICamera[] Cameras { get; }
    }

    public delegate void CameraListUShortInputSigDelegate(UShortInputSig uShortInputSig, ICameraList cameraList);
    public delegate void CameraListStringInputSigDelegate(StringInputSig stringInputSig, ICameraList cameraList);

    /// <summary>
    /// selection of camera and status of camara selected
    /// </summary>
    internal class CameraList : ICameraList, IDisposable
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
                public const uint SetSelectedCameraIndex = 2;
                public const uint StateOfSelectedCamera = 3;
                public const uint ErrorCodeOfSelectedCamera = 4;
                public const uint RetryCountOfSelectedCamera = 5;
                public const uint StatusOfSelectedCameraImage = 6;

                public const uint NumberOfCameras = 1;
                public const uint IndexOfSelectedCamera = 2;
            }
            internal class Strings
            {
                public const uint ErrorMessageOfSelectedCamera = 1;
                public const uint ResolutionOfSelectedCamera = 2;
                public const uint LastUpdateTimeOfSelectedCameraImage = 3;
                public const uint URLOfVideo = 4;
                public const uint SourceTypeOfSelectedCamara = 5;
                public const uint ImageURLOfSelectedCamera = 6;
                public const uint VideoURLOfSelectedCamera = 7;

            }
        }

        #endregion

        #region Construction and Initialization

        internal CameraList(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private readonly IDictionary<uint, List<uint>> _camerasSmartObjectIdMappings = new Dictionary<uint, List<uint>> { { 82, new List<uint> { 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112 } } };

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.SetSelectedCameraIndex, onSetSelectedCameraIndex);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.StateOfSelectedCamera, onStateOfSelectedCamera);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.ErrorCodeOfSelectedCamera, onErrorCodeOfSelectedCamera);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.RetryCountOfSelectedCamera, onRetryCountOfSelectedCamera);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.StatusOfSelectedCameraImage, onStatusOfSelectedCameraImage);
            ComponentMediator.ConfigureStringEvent(controlJoinId, Joins.Strings.ErrorMessageOfSelectedCamera, onErrorMessageOfSelectedCamera);
            ComponentMediator.ConfigureStringEvent(controlJoinId, Joins.Strings.ResolutionOfSelectedCamera, onResolutionOfSelectedCamera);
            ComponentMediator.ConfigureStringEvent(controlJoinId, Joins.Strings.LastUpdateTimeOfSelectedCameraImage, onLastUpdateTimeOfSelectedCameraImage);
            ComponentMediator.ConfigureStringEvent(controlJoinId, Joins.Strings.URLOfVideo, onURLOfVideo);
            ComponentMediator.ConfigureStringEvent(controlJoinId, Joins.Strings.SourceTypeOfSelectedCamara, onSourceTypeOfSelectedCamara);
            ComponentMediator.ConfigureStringEvent(controlJoinId, Joins.Strings.ImageURLOfSelectedCamera, onImageURLOfSelectedCamera);
            ComponentMediator.ConfigureStringEvent(controlJoinId, Joins.Strings.VideoURLOfSelectedCamera, onVideoURLOfSelectedCamera);

            List<uint> camerasList = _camerasSmartObjectIdMappings[controlJoinId];
            Cameras = new ICamera[camerasList.Count];
            for (int index = 0; index < camerasList.Count; index++)
            {
                Cameras[index] = new Camera(ComponentMediator, camerasList[index]); 
            }

        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
            for (int index = 0; index < Cameras.Length; index++)
            {
                ((Camera)Cameras[index]).AddDevice(device);
            }
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
            for (int index = 0; index < Cameras.Length; index++)
            {
                ((Camera)Cameras[index]).RemoveDevice(device);
            }
        }

        #endregion

        #region CH5 Contract

        public ICamera[] Cameras { get; private set; }

        public event EventHandler<UIEventArgs> SetSelectedCameraIndex;
        private void onSetSelectedCameraIndex(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = SetSelectedCameraIndex;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> StateOfSelectedCamera;
        private void onStateOfSelectedCamera(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = StateOfSelectedCamera;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> ErrorCodeOfSelectedCamera;
        private void onErrorCodeOfSelectedCamera(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = ErrorCodeOfSelectedCamera;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> RetryCountOfSelectedCamera;
        private void onRetryCountOfSelectedCamera(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = RetryCountOfSelectedCamera;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> StatusOfSelectedCameraImage;
        private void onStatusOfSelectedCameraImage(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = StatusOfSelectedCameraImage;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void NumberOfCameras(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.NumberOfCameras], this);
            }
        }

        public void IndexOfSelectedCamera(CameraListUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.IndexOfSelectedCamera], this);
            }
        }

        public event EventHandler<UIEventArgs> ErrorMessageOfSelectedCamera;
        private void onErrorMessageOfSelectedCamera(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = ErrorMessageOfSelectedCamera;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> ResolutionOfSelectedCamera;
        private void onResolutionOfSelectedCamera(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = ResolutionOfSelectedCamera;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> LastUpdateTimeOfSelectedCameraImage;
        private void onLastUpdateTimeOfSelectedCameraImage(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = LastUpdateTimeOfSelectedCameraImage;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> URLOfVideo;
        private void onURLOfVideo(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = URLOfVideo;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> SourceTypeOfSelectedCamara;
        private void onSourceTypeOfSelectedCamara(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = SourceTypeOfSelectedCamara;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> ImageURLOfSelectedCamera;
        private void onImageURLOfSelectedCamera(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = ImageURLOfSelectedCamera;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> VideoURLOfSelectedCamera;
        private void onVideoURLOfSelectedCamera(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = VideoURLOfSelectedCamera;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
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

            for (int index = 0; index < Cameras.Length; index++)
            {
                ((Camera)Cameras[index]).Dispose();
            }

            SetSelectedCameraIndex = null;
            StateOfSelectedCamera = null;
            ErrorCodeOfSelectedCamera = null;
            RetryCountOfSelectedCamera = null;
            StatusOfSelectedCameraImage = null;
            ErrorMessageOfSelectedCamera = null;
            ResolutionOfSelectedCamera = null;
            LastUpdateTimeOfSelectedCameraImage = null;
            URLOfVideo = null;
            SourceTypeOfSelectedCamara = null;
            ImageURLOfSelectedCamera = null;
            VideoURLOfSelectedCamera = null;
        }

        #endregion

    }
}
