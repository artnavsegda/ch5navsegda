using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using CH5SampleSSP.Lighting;

namespace CH5SampleSSP
{
    public class DemoLightingLogic
    {

        /// <summary>
        /// This class simulates programing of scenes as well as on/off functionality for sample lights. 
        /// Feedback for Lighing page from UI are programmed in the events below 
        /// </summary>

        private ILightingList _allLights;

        public DemoLightingLogic(ILightingList allSystemLights)
        {
            var lights = GetLightLoads();

            allSystemLights.Scene_0 += LightingScene_AllOn;
            allSystemLights.Scene_1 += LightingScene_AllOff;
            allSystemLights.Scene_2 += LightingScene_AllHigh;
            allSystemLights.Scene_3 += LightingScene_AllMedium;
            allSystemLights.Scene_4 += LightingScene_AllLow;

            for (int i = 0; i < allSystemLights.DimmableLoad.Length; i++)
            {
                allSystemLights.DimmableLoad[i].UserObject = lights[i];
                lights[0].UserSpecifiedObject = allSystemLights.DimmableLoad[i];

                IDimmableLoad lightingload = allSystemLights.DimmableLoad[i];

                lightingload.On += Light_On;
                lightingload.Off += Light_Off;
                lightingload.Level += Light_Level;
            }

            _allLights = allSystemLights;
        }

        private class LightDevice
        {
            public string LoadName { get; set; }
            public object UserSpecifiedObject { get; set; }

            public LightDevice() { }
        }

        private List<LightDevice> GetLightLoads()
        {
            var l = new List<LightDevice>()
            {
                new LightDevice() {LoadName = "Light1"},
                new LightDevice() {LoadName = "Light2"},
                new LightDevice() {LoadName = "Light3"},
                new LightDevice() {LoadName = "Light4"},
                new LightDevice() {LoadName = "Light5"},
                new LightDevice() {LoadName = "Light6"},
                new LightDevice() {LoadName = "Light7"},
                new LightDevice() {LoadName = "Light8"}
            };
            return l;
        }

        private void LightingScene_AllOn(object sender, UIEventArgs e)
        {

            ushort levelAllValue = SimplSharpDeviceHelper.PercentToUshort(100);

            for (int i = 0; i < _allLights.DimmableLoad.Length; i++)
            {
                _allLights.DimmableLoad[i].Level_Feedback((sig, component) => { sig.UShortValue = levelAllValue; });
            }
        }

        private void LightingScene_AllOff(object sender, UIEventArgs e)
        {

            ushort levelAllOffValue = SimplSharpDeviceHelper.PercentToUshort(0);

            for (int i = 0; i < _allLights.DimmableLoad.Length; i++)
            {
                _allLights.DimmableLoad[i].Level_Feedback((sig, component) => { sig.UShortValue = levelAllOffValue; });
            }
        }

        private void LightingScene_AllHigh(object sender, UIEventArgs e)
        {

            _allLights.DimmableLoad[0].Level_Feedback((sig, component) => { sig.UShortValue = 55555; });
            _allLights.DimmableLoad[1].Level_Feedback((sig, component) => { sig.UShortValue = 50000; });
            _allLights.DimmableLoad[2].Level_Feedback((sig, component) => { sig.UShortValue = 48900; });
            _allLights.DimmableLoad[3].Level_Feedback((sig, component) => { sig.UShortValue = 47100; });
            _allLights.DimmableLoad[4].Level_Feedback((sig, component) => { sig.UShortValue = 60000; });
            _allLights.DimmableLoad[5].Level_Feedback((sig, component) => { sig.UShortValue = 64000; });
            _allLights.DimmableLoad[6].Level_Feedback((sig, component) => { sig.UShortValue = 48900; });
            _allLights.DimmableLoad[7].Level_Feedback((sig, component) => { sig.UShortValue = 50000; });
            _allLights.DimmableLoad[8].Level_Feedback((sig, component) => { sig.UShortValue = 55555; });

        }

        private void LightingScene_AllMedium(object sender, UIEventArgs e)
        {

            _allLights.DimmableLoad[0].Level_Feedback((sig, component) => { sig.UShortValue = 32000; });
            _allLights.DimmableLoad[1].Level_Feedback((sig, component) => { sig.UShortValue = 29123; });
            _allLights.DimmableLoad[2].Level_Feedback((sig, component) => { sig.UShortValue = 35321; });
            _allLights.DimmableLoad[3].Level_Feedback((sig, component) => { sig.UShortValue = 28921; });
            _allLights.DimmableLoad[4].Level_Feedback((sig, component) => { sig.UShortValue = 30000; });
            _allLights.DimmableLoad[5].Level_Feedback((sig, component) => { sig.UShortValue = 64000; });
            _allLights.DimmableLoad[6].Level_Feedback((sig, component) => { sig.UShortValue = 48900; });
            _allLights.DimmableLoad[7].Level_Feedback((sig, component) => { sig.UShortValue = 50000; });
            _allLights.DimmableLoad[8].Level_Feedback((sig, component) => { sig.UShortValue = 55555; });

        }

        private void LightingScene_AllLow(object sender, UIEventArgs e)
        {

            _allLights.DimmableLoad[0].Level_Feedback((sig, component) => { sig.UShortValue = 2000; });
            _allLights.DimmableLoad[1].Level_Feedback((sig, component) => { sig.UShortValue = 12000; });
            _allLights.DimmableLoad[2].Level_Feedback((sig, component) => { sig.UShortValue = 5000; });
            _allLights.DimmableLoad[3].Level_Feedback((sig, component) => { sig.UShortValue = 8921; });
            _allLights.DimmableLoad[4].Level_Feedback((sig, component) => { sig.UShortValue = 12000; });
            _allLights.DimmableLoad[5].Level_Feedback((sig, component) => { sig.UShortValue = 3456; });
            _allLights.DimmableLoad[6].Level_Feedback((sig, component) => { sig.UShortValue = 2000; });
            _allLights.DimmableLoad[7].Level_Feedback((sig, component) => { sig.UShortValue = 12000; });
            _allLights.DimmableLoad[8].Level_Feedback((sig, component) => { sig.UShortValue = 5000; });

        }

        private void Light_On(object sender, UIEventArgs e)
        {
            ushort levelValue = SimplSharpDeviceHelper.PercentToUshort(100);
            ((DimmableLoad)sender).Level_Feedback((sig, component) => { sig.UShortValue = levelValue; });
        }

        private void Light_Off(object sender, UIEventArgs e)
        {
            ushort levelValue = SimplSharpDeviceHelper.PercentToUshort(0);
            ((DimmableLoad)sender).Level_Feedback((sig, component) => { sig.UShortValue = levelValue; });
        }

        private void Light_Level(object sender, UIEventArgs e)
        {
            ((DimmableLoad)sender).Level_Feedback((sig, component) => { sig.UShortValue = e.SigArgs.Sig.UShortValue; });
        }
    }
}