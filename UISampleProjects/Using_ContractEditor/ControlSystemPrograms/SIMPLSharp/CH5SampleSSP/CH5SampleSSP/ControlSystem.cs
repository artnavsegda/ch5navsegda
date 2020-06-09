using System;
using System.Collections.Generic;
using System.Linq;
using CH5SampleSSP.Video;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport; // For Generic Device Support
using Crestron.SimplSharpPro.UI;
using CH5SampleSSP.Video;
using CH5SampleSSP.Lighting;


namespace CH5SampleSSP
{
    public class ControlSystem : CrestronControlSystem
    {
        public Contract Touchscreen1060Contract;
        public DemoVideoCameraLogic Touchscreen1060VideoCamera;

        public Contract Touchscreen760Contract;
        public DemoVideoCameraLogic Touchscreen760VideoCamera;
        
        public Contract Touchscreen560Contract;
        public DemoVideoCameraLogic Touchscreen560VideoCamera;

        public Contract Touchscreen560PContract;
        public DemoVideoCameraLogic Touchscreen560PVideoCamera;
        
        public Contract MobileAppContract;
        public DemoVideoCameraLogic MobileAppVideoCamera;

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(ControlSystem_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(ControlSystem_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(ControlSystem_ControllerEthernetEventHandler);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

 
        public override void InitializeSystem()
        {
            try
            {
                
                // List of all Touchscreens in system
                IList<BasicTriListWithSmartObject> panelList = new List<BasicTriListWithSmartObject>();

                Tsw1060 touchScreen1060 = new Tsw1060(0x0C, this);
                touchScreen1060.Register();

                Tsw760 touchScreen760 = new Tsw760(0x0A, this);
                touchScreen760.Register();

                Tsw560 touchScreen560 = new Tsw560(0x0E, this);
                touchScreen560.Register();

                Tsw560P touchScreen560P = new Tsw560P(0x0D, this);
                touchScreen560P.Register();

                CrestronApp mobileApp = new CrestronApp(0x0B, this);
                mobileApp.ParameterProjectName.Value = "ch5-sample-project";
                mobileApp.Register();

                panelList.Add(touchScreen1060);
                panelList.Add(touchScreen760);
                panelList.Add(touchScreen560);
                panelList.Add(touchScreen560P);
                panelList.Add(mobileApp);
 
                // ---------------------------------------------------------------------------------------------------------------
                // Create Contract Instance to program all touchscreens in system. This Contract Reference will globablly program
                // Lights, Source Selection, and ContactList
                // ---------------------------------------------------------------------------------------------------------------

                // Contract Instance
                var globalContract = new Contract(panelList.ToArray());

                // Sample Programming of Lights/Scenes for all Touchscreens
                var demoLightingLogic = new DemoRoomLogic(globalContract.Room);

                // Sample Programming of Contact List for all Touchscreens
                var demoContactListingLogic = new DemoContactListingLogic(globalContract.ContactList);

                // Sample Programming of Source Selection for all Touchscreens
                var demoSourceSelectionLogic = new DemoSourceSelectLogic(globalContract.SourceList);


                // --------------------------------------------------------------------------------------------------------------
                // Creating Multiple Contract Instances for each Touchscreen to program VIDEO CAMERA PAGE.  
                // This will allow each Touchscreen Device to independantly play a different video camera.  
                // For example TSW1060 can watch camera 2 while TSW760 can watch camera 1 and so on.
                // --------------------------------------------------------------------------------------------------------------

                //Contract Instance for TSW1060
                Touchscreen1060Contract     = new Contract(touchScreen1060);
                Touchscreen1060VideoCamera  = new DemoVideoCameraLogic(Touchscreen1060Contract.CameraList);
                              
                // Contact Instance for TSW760
                Touchscreen760Contract     = new Contract(touchScreen760);
                Touchscreen760VideoCamera  = new DemoVideoCameraLogic(Touchscreen760Contract.CameraList);

                // Contact Instance for TSW560
                Touchscreen560Contract = new Contract(touchScreen560);
                Touchscreen560VideoCamera = new DemoVideoCameraLogic(Touchscreen560Contract.CameraList);
                               
                // Contact Instance for TSW560P
                Touchscreen560PContract = new Contract(touchScreen560P);
                Touchscreen560PVideoCamera = new DemoVideoCameraLogic(Touchscreen560PContract.CameraList);

                // Contact Instance for MobileApp
                MobileAppContract = new Contract(mobileApp);
                MobileAppVideoCamera = new DemoVideoCameraLogic(MobileAppContract.CameraList);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        void ControlSystem_ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void ControlSystem_ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void ControlSystem_ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;
                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;
                case (eSystemEventType.Rebooting):
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }

        }
    }
}