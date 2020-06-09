using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using CH5SampleSSP.Selector;

namespace CH5SampleSSP
{
    public class DemoSourceSelectLogic
    {
        /// <summary>
        /// This class simulates programing of Source List selection for all Touchscreens globally. 
        /// </summary>

        private ISourceList _allSources;
        private List<SourceItem> _allSourceItems;

        public DemoSourceSelectLogic(ISourceList roomSources)
        {
            
            var availableSources = GetSources();

            // Send total number of sources to UI
            roomSources.NumberOfSources((sig, component) => { sig.UShortValue = GetNumberOfSources(); });

            _allSources = roomSources;
            _allSourceItems = availableSources;

            // On initialize clears all selected sources
            ClearSourceSelection();
            
            // Loops all sources and sents serial values to UI
            for (int i = 0; i < 4; i++)
            {
                roomSources.Sources[i].UserObject = availableSources[i];
                availableSources[i].UserSpecifiedObject = roomSources.Sources[i];

                roomSources.Sources[i].NameOfSource((sig, component) => { sig.StringValue = availableSources[i].Name; });
                roomSources.Sources[i].IconClassOfSource((sig, component) => { sig.StringValue = availableSources[i].IconClass; });

                // Event called when a source is clicked on in UI
                roomSources.Sources[i].SetSourceSelected += SourceDevice_Selected;
            }
        }

        // Example class for Source Items 
        private class SourceItem
        {
            public string Name { get; set; }
            public string IconClass { get; set; }
            public bool IsSelected { get; set; }
            public object UserSpecifiedObject { get; set; }

            public SourceItem() { }
        }
    
        // Collection of Sources 
        private List<SourceItem> GetSources()
        {
            var c = new List<SourceItem>()
            {
                new SourceItem(){ Name="HDMI", IconClass = "fas fa-tv", IsSelected = false},
                new SourceItem(){ Name="Airboard",IconClass = "fas fa-chalkboard", IsSelected = false},
                new SourceItem(){ Name="Laptop", IconClass = "fas fa-laptop", IsSelected = false},
                new SourceItem(){ Name="AirMedia",IconClass = "fas fa-wifi", IsSelected = false}
            };
            return c;
        }

        // Sets number of available sources to display on UI 
        private static ushort GetNumberOfSources()
        {
            return 4;
        }

        // Clears all selected sources
        private void ClearSourceSelection()
        {
            for (int j = 0; j < 4; j++)
            {
                _allSources.Sources[j].SourceIsSelected((sig, component) => { sig.BoolValue = false; });
            }

            for (int i = 0; i < _allSourceItems.Count; i++)
            {
                _allSourceItems[i].IsSelected = false;
            }
        }
       
        // When source is clicked on in UI Feedback of True is sent back 
        private void SourceDevice_Selected(object sender, UIEventArgs e)
        {

            if (e.SigArgs.Sig.BoolValue)
            {
                ClearSourceSelection();

                SourceItem source = (SourceItem)((Selector.Source)sender).UserObject;
                source.IsSelected = true;

                ((Selector.Source)sender).SourceIsSelected((sig, component) => { sig.BoolValue = source.IsSelected; });          
            }
        }
    }
}