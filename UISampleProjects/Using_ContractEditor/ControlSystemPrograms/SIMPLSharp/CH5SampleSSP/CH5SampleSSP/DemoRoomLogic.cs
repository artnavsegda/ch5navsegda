using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Crestron.SimplSharp;
using CH5SampleSSP.Lighting;
using Crestron.SimplSharpPro;
using System.Threading;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DeviceSupport;
using Thread = System.Threading.Thread;

namespace CH5SampleSSP
{
    public class DemoRoomLogic
    {
        /// <summary>
        /// This class simulates programing of Lighting Room for all Touchscreens globallyThis sample room has Dimmingloads and Scenes 
        /// Feedback for Light page from UI are programmed in the events below 
        /// </summary>

        private IRoom allRoomLights;
        private List<LightDevice> _allLightDevices;
        public List<CTimer> _cTimers; // Using cTimer for demo purposes, when programing Lighting Devices it is recommended to use Ramping object


        public DemoRoomLogic(IRoom room)
        {
            var systemLights = GetLightLoads();
            var systemScenes = GetRoomScenes();

            _allLightDevices = systemLights;

            // Send total number of Lights to UI
            room.NumberOfLights((sig, component) => { sig.UShortValue = GetNumberOfLights(); });

            // Send total number of Scenes to UI
            room.NumberOfScenes((sig, component) => { sig.UShortValue = GetNumberOfScenes(); });
            room.NameOfRoom((sig, component) => { sig.StringValue = "Great Room"; });

            // Room Lights
            for (int i = 0; i < 6; i++)
            {
                room.DimmableLights[i].UserObject = systemLights[i];
                systemLights[i].UserSpecifiedObject = room.DimmableLights[i];
                
                room.DimmableLights[i].NameOfLight((sig, component) => { sig.StringValue = systemLights[i].LoadName; });
                room.DimmableLights[i].LightIsAtLevel((sig, component) => { sig.UShortValue = systemLights[i].LoadLevel; });

                // Event called when Light On button is clicked on in UI
                room.DimmableLights[i].TurnLightOn += LightOn;
                room.DimmableLights[i].TurnLightOff += LightOff;
                room.DimmableLights[i].SetLightLevel += LightLevel;
            }

            // Room Scenes 
            for (int j = 0; j < 5; j++)
            {
                room.Scenes[j].UserObject = systemScenes[j];
                systemScenes[j].UserSpecifiedObject = room.Scenes[j];
   
                room.Scenes[j].NameOfScene((sig, component) => { sig.StringValue = systemScenes[j].SceneName; });
                room.Scenes[j].TriggerSceneStart += TriggerScene;
            }

            allRoomLights = room;

            _cTimers = new List<CTimer>();

        }

        // Example Class Room Lights 
        private class LightDevice
        {
            public string LoadName { get; set; }
            public ushort LoadLevel { get; set; }
            public object UserSpecifiedObject { get; set; }

            public LightDevice() { }
        }

        private List<LightDevice> GetLightLoads()
        {
            var l = new List<LightDevice>()
            {
                // Defaulting sliders to a set level
                new LightDevice() {LoadName = "Lamp", LoadLevel = 20000},
                new LightDevice() {LoadName = "Sconces", LoadLevel = 30000},
                new LightDevice() {LoadName = "Downlights", LoadLevel = 12345},
                new LightDevice() {LoadName = "Media Wall", LoadLevel = 54321},
                new LightDevice() {LoadName = "Tracks", LoadLevel = 234},
                new LightDevice() {LoadName = "Pendants", LoadLevel = 60000}
            };
            return l;
        }

        // Example Class for Room Scene 
        private class RoomScene
        {
            public string SceneName { get; set; }
            public object UserSpecifiedObject { get; set; }

            public RoomScene() { }
        }

        private List<RoomScene> GetRoomScenes()
        {
            var k = new List<RoomScene>()
            {
                new RoomScene() {SceneName = "ALL ON"},
                new RoomScene() {SceneName = "ALL OFF"},
                new RoomScene() {SceneName = "HIGH"},
                new RoomScene() {SceneName = "MEDIUM"},
                new RoomScene() {SceneName = "LOW"}

            };
            return k;
        }

        // returns total number of Dimmingloads used in UI
        private ushort GetNumberOfLights()
        {
            return 6;
        }

        // returns total number of scenes used in UI
        private ushort GetNumberOfScenes()
        {
            return 5;
        }

        // Event to turn on selected light 
        private void LightOn(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                LightDevice light = (LightDevice)((DimmableLight) sender).UserObject;

                // using ctimer for demo.  When using Light Devices please use Ramping object
              //  _cTimer1 = new CTimer(o => SimulateOnOffLightRamp(sender, "On"), 500);

                // using ctimer for demo.  When using Light Devices please use Ramping object
                _cTimers.Add(new CTimer(o => SimulateOnOffLightRamp(sender, "On"), 300));
            }
        }

        // Event to turn off selected light 
        private void LightOff(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                LightDevice light = (LightDevice)((DimmableLight)sender).UserObject;

                // using ctimer for demo.  When using Light Devices please use Ramping object
                // _cTimer1 = new CTimer(o => SimulateOnOffLightRamp(sender, "Off"), 500);
                // using ctimer for demo.  When using Light Devices please use Ramping object
                _cTimers.Add(new CTimer(o => SimulateOnOffLightRamp(sender, "Off"), 300));
            }
        }

        // using ctimer to simulate ramping of slider when light is turn on/off
        public void SimulateOnOffLightRamp(object sender, string status)
        {
            LightDevice light = (LightDevice)((DimmableLight)sender).UserObject;

            float vOut = SimplSharpDeviceHelper.UshortToPercent(light.LoadLevel);

            ushort newLevelValue;

            if (status == "On")
            {
                for (float i = vOut; i < 100; i++)
                {
                    ushort levelValue = SimplSharpDeviceHelper.PercentToUshort(i);
                    ((DimmableLight)sender).LightIsAtLevel((sig, component) => { sig.UShortValue = levelValue; });
                }

                newLevelValue = SimplSharpDeviceHelper.PercentToUshort(100);
                light.LoadLevel = newLevelValue;
            }
            else
            {
                for (float i = vOut; i > 0; i--)
                {
                    ushort levelValue = SimplSharpDeviceHelper.PercentToUshort(i);
                    ((DimmableLight)sender).LightIsAtLevel((sig, component) => { sig.UShortValue = levelValue; });
                }

                newLevelValue = SimplSharpDeviceHelper.PercentToUshort(0);
                light.LoadLevel = newLevelValue;
            }
        }

        // Event to set selected light level
        private void LightLevel(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                LightDevice light = (LightDevice)((DimmableLight)sender).UserObject;

                ((DimmableLight)sender).LightIsAtLevel((sig, component) => { sig.UShortValue = e.SigArgs.Sig.UShortValue; });
            }
        }

        // using ctimer to simulate ramping of slider scene is Selected on UI
        public void SimulateLoadRamp(IRoom allRoomRef, int lightIndex, float rampTo)
        {
            LightDevice light = _allLightDevices[lightIndex];
            float light1Level = SimplSharpDeviceHelper.UshortToPercent(light.LoadLevel);

            ushort levelValue1;
            
            for (float i = light1Level; i < rampTo; i++)
            {
                levelValue1 = SimplSharpDeviceHelper.PercentToUshort(i);
                
                allRoomLights.DimmableLights[lightIndex].LightIsAtLevel((sig, component) => { sig.UShortValue = levelValue1; });

                light.LoadLevel = levelValue1;
            }

            for (float i = light1Level; i > rampTo; i--)
            {
                levelValue1 = SimplSharpDeviceHelper.PercentToUshort(i);
                
                allRoomLights.DimmableLights[lightIndex].LightIsAtLevel((sig, component) => { sig.UShortValue = levelValue1; });

                light.LoadLevel = levelValue1;
            }
        }

        // Event to set all lights based on selected scene
        private void TriggerScene(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                RoomScene activeScene = (RoomScene)((Scene)sender).UserObject;

                switch (activeScene.SceneName)
                {
                    case "ALL ON":
                        // DEBUG CrestronConsole.PrintLine("All on pressed");
                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 0, 100), 300));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 1, 100), 500));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 2, 100), 800));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 3, 100), 1100));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 4, 100), 1400));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 5, 100), 1700));
                        
                        break;

                    case "ALL OFF":
                        // DEBUG CrestronConsole.PrintLine("All Off pressed");
                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 0, 0), 300));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 1, 0), 500));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 2, 0), 800));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 3, 0), 1100));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 4, 0), 1400));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 5, 0), 1700));
                        
                        break;

                    case "HIGH":
                         // DEBUG -> CrestronConsole.PrintLine("High pressed");
                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 0, 85), 300));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 1, 76), 500));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 2, 74), 800));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 3, 71), 1100));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 4, 91), 1400));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 5, 97), 1700));
                        
                        break;

                    case "MEDIUM":
                        // DEBUG -> CrestronConsole.PrintLine("Medium pressed");
                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 0, 49), 300));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 1, 44), 500));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 2, 54), 800));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 3, 44), 1100));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 4, 46), 1400));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 5, 60), 1700));
                        
                        break;

                    case "LOW":
                        // DEBUG -> CrestronConsole.PrintLine("Low pressed");
                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 0, 3), 300));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 1, 18), 500));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 2, 7), 800));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 3, 13), 1100));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 4, 18), 1400));

                        _cTimers.Add(new CTimer(o => SimulateLoadRamp(allRoomLights, 5, 5), 1700));
                        
                        break;
                }
            }
        }
    }
}