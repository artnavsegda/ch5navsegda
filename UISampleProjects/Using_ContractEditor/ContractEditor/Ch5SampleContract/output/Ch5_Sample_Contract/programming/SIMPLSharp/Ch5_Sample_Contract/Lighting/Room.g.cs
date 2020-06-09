using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Lighting
{
    /// <summary>
    /// number of active dimmers
    /// </summary>
    /// <summary>
    /// number of scenes available in this room
    /// </summary>
    /// <summary>
    /// the name of this room as seen by user
    /// </summary>
    public interface IRoom
    {
        object UserObject { get; set; }

        void NumberOfLights(RoomUShortInputSigDelegate callback);
        void NumberOfScenes(RoomUShortInputSigDelegate callback);
        void NameOfRoom(RoomStringInputSigDelegate callback);

        Ch5_Sample_Contract.Lighting.IDimmableLight[] DimmableLights { get; }
        Ch5_Sample_Contract.Lighting.IScene[] Scenes { get; }
    }

    public delegate void RoomUShortInputSigDelegate(UShortInputSig uShortInputSig, IRoom room);
    public delegate void RoomStringInputSigDelegate(StringInputSig stringInputSig, IRoom room);

    /// <summary>
    /// Room contains lights and scenes
    /// </summary>
    public class Room : IRoom, IDisposable
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
            internal class Numerics
            {

                public const uint NumberOfLights = 1;
                public const uint NumberOfScenes = 2;
            }
            internal class Strings
            {
                public const uint NameOfRoom = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Room(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            Initialize(devices, controlJoinId);
        }

        internal Room(BasicTriListWithSmartObject device, uint controlJoinId)
            : this(new [] { device }, controlJoinId)
        {
        }

        private readonly IDictionary<uint, List<uint>> _dimmableLightsSmartObjectIdMappings = new Dictionary<uint, List<uint>> { { 41, new List<uint> { 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71 } } };
        private readonly IDictionary<uint, List<uint>> _scenesSmartObjectIdMappings = new Dictionary<uint, List<uint>> { { 41, new List<uint> { 72, 73, 74, 75, 76, 77, 78, 79, 80, 81 } } };

        private void Initialize(BasicTriListWithSmartObject[] devices, uint controlJoinId)
        {
            if (_devices == null)
            {
                ControlJoinId = controlJoinId; 
 
                _devices = new List<BasicTriListWithSmartObject>(); 
 
                
                List<uint> dimmableLightsList = _dimmableLightsSmartObjectIdMappings[controlJoinId];
                DimmableLights = new Ch5_Sample_Contract.Lighting.IDimmableLight[dimmableLightsList.Count];
                for (int index = 0; index < dimmableLightsList.Count; index++)
                {
                    DimmableLights[index] = new Ch5_Sample_Contract.Lighting.DimmableLight(devices, dimmableLightsList[index]); 
                }
                
                List<uint> scenesList = _scenesSmartObjectIdMappings[controlJoinId];
                Scenes = new Ch5_Sample_Contract.Lighting.IScene[scenesList.Count];
                for (int index = 0; index < scenesList.Count; index++)
                {
                    Scenes[index] = new Ch5_Sample_Contract.Lighting.Scene(devices, scenesList[index]); 
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
            for (int index = 0; index < DimmableLights.Length; index++)
            {
                ((Ch5_Sample_Contract.Lighting.DimmableLight)DimmableLights[index]).AddDevice(device);
            }
            for (int index = 0; index < Scenes.Length; index++)
            {
                ((Ch5_Sample_Contract.Lighting.Scene)Scenes[index]).AddDevice(device);
            }
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.Instance.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
            for (int index = 0; index < DimmableLights.Length; index++)
            {
                ((Ch5_Sample_Contract.Lighting.DimmableLight)DimmableLights[index]).RemoveDevice(device);
            }
            for (int index = 0; index < Scenes.Length; index++)
            {
                ((Ch5_Sample_Contract.Lighting.Scene)Scenes[index]).RemoveDevice(device);
            }
        }

        #endregion

        #region CH5 Contract

        public Ch5_Sample_Contract.Lighting.IDimmableLight[] DimmableLights { get; private set; }

        public Ch5_Sample_Contract.Lighting.IScene[] Scenes { get; private set; }


        public void NumberOfLights(RoomUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.NumberOfLights], this);
            }
        }

        public void NumberOfScenes(RoomUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.NumberOfScenes], this);
            }
        }

        public void NameOfRoom(RoomStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.NameOfRoom], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "Room", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            for (int index = 0; index < DimmableLights.Length; index++)
            {
                ((Ch5_Sample_Contract.Lighting.DimmableLight)DimmableLights[index]).Dispose();
            }
            for (int index = 0; index < Scenes.Length; index++)
            {
                ((Ch5_Sample_Contract.Lighting.Scene)Scenes[index]).Dispose();
            }

        }

        #endregion

    }
}
