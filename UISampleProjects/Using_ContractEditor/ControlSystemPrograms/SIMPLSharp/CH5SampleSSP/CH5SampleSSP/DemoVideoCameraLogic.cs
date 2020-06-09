using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CH5SampleSSP.Video;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;

namespace CH5SampleSSP
{
    public class DemoVideoCameraLogic
    {
        /// <summary>
        /// This class simulates programing of Video Camera page for each Touchscreens independly. 
        //  This will allow each Touchscreen Device to independantly play a different video camera.  
        //  For example TSW1060 can watch camera 2 while TSW760 can watch camera 1 and so on.
        /// </summary>
        /// 
        /// 
        private ICameraList _cameraListItems;

        public DemoVideoCameraLogic(ICameraList cameras)
        {
            var availableCameras = GetVideoCameras();

            _cameraListItems = cameras;
           
            // Send total number of Cameras to UI
            cameras.NumberOfCameras((sig, component) => { sig.UShortValue = GetNumberOfCameras(); });

            // On initialize set Camera 1 as default
            cameras.IndexOfSelectedCamera((sig, component) => { sig.UShortValue = DefaultSelectedIndex(); });

            // Event called when camera is selected on UI
            cameras.SetSelectedCameraIndex += SelectedCamera;
           
            // Sends information for each camera to UI
            for (int i = 0; i < 4; i++)
            {
              //  Debug: CrestronConsole.PrintLine("Camera Name ----->" + availableCameras[i].CameraName);

               cameras.Cameras[i].UserObject = availableCameras[i];
               availableCameras[i].UserSpecifiedObject = cameras.Cameras[i];

               cameras.Cameras[i].NameOfCamera((sig, component) => { sig.StringValue = availableCameras[i].CameraName; });
               cameras.Cameras[i].VideoURLOfCamera((sig, component) => { sig.StringValue = availableCameras[i].CameraUrl; });
               cameras.Cameras[i].SourceTypeOfCamera((sig, component) => { sig.StringValue = availableCameras[i].CameraSourceType; });
               cameras.Cameras[i].UserIdOfVideo((sig, component) => { sig.StringValue = availableCameras[i].VideoUserId; });
               cameras.Cameras[i].PasswordOfVideo((sig, component) => { sig.StringValue = availableCameras[i].VideoPassword; });
               cameras.Cameras[i].ImageURLOfCamera((sig, component) => { sig.StringValue = availableCameras[i].CameraImageUrl; });
               cameras.Cameras[i].TimePeriodOfImage((sig, component) => { sig.UShortValue = availableCameras[i].ImageTimePeriod; });
               cameras.Cameras[i].UserIdOfImage((sig, component) => { sig.StringValue = availableCameras[i].ImageUserId; });
               cameras.Cameras[i].PasswordOfImage((sig, component) => { sig.StringValue = availableCameras[i].ImagePassword; });
            }
        }

        // Video Camera Class
        public class VideoCamera
        {
            public string CameraName { get; set; }
            public string CameraUrl { get; set; }
            public string CameraSourceType { get; set; }
            public string VideoUserId { get; set; }
            public string VideoPassword { get; set; }
            public string CameraImageUrl { get; set; }
            public ushort ImageTimePeriod { get; set; }
            public string ImageUserId { get; set; }
            public string ImagePassword { get; set; }
            public object UserSpecifiedObject { get; set; }

            public VideoCamera() { }
        }

        // Returns list of available cameras
        public List<VideoCamera> GetVideoCameras()
        {
            var c = new List<VideoCamera>()
            {
                new VideoCamera(){ CameraName="Camera 1", CameraUrl="rtsp://admin:Welcome2C$@10.88.17.10", CameraSourceType="Network", VideoUserId="admin", VideoPassword="Welcome2C$", CameraImageUrl="http://10.88.17.10/cgi-bin/snapshot.cgi", ImageTimePeriod=10, ImageUserId="admin", ImagePassword="Welcome2C$"},
                new VideoCamera(){ CameraName="Camera 2", CameraUrl="rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov", CameraSourceType="Network", VideoUserId="", VideoPassword="", CameraImageUrl="", ImageTimePeriod= 0 , ImageUserId="", ImagePassword=""},
                new VideoCamera(){ CameraName="Camera 3", CameraUrl="rtsp://admin:Welcome2C$@10.88.24.235", CameraSourceType="Network", VideoUserId="admin", VideoPassword="Welcome2C$", CameraImageUrl="http://10.88.24.235/cgi-bin/snapshot.cgi?channel=1&subtype=1", ImageTimePeriod=10, ImageUserId="admin", ImagePassword="Welcome2C$"},
                new VideoCamera(){ CameraName="Camera 4", CameraUrl="rtsp://admin:Welcome2C$@10.88.17.12", CameraSourceType="Network", VideoUserId="admin", VideoPassword="Welcome2C$", CameraImageUrl="http://10.88.17.12/Streaming/channels/1/picture", ImageTimePeriod=10, ImageUserId="admin", ImagePassword="Welcome2C$"}
            };
            return c;
        }

        // Sets number of available Cameras to display on UI 
        public ushort GetNumberOfCameras()
        {
            return 4;
        }

        // Clears all selected cameras
        public ushort DefaultSelectedIndex()
        {
            return 0;
        }
        
        // Sends Video Camera feedback to specific touchscreen
        public void SelectedCamera(object sender, UIEventArgs e)
        {
            CrestronConsole.PrintLine("Debug (Selected index)  " + e.SigArgs.Sig.UShortValue);

            _cameraListItems.IndexOfSelectedCamera((sig, component) => { sig.UShortValue = e.SigArgs.Sig.UShortValue; });

        }
    }
}