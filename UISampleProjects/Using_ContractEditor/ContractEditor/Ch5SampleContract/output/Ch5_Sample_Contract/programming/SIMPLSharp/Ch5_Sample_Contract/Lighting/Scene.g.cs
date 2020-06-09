using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Ch5_Sample_Contract.Lighting
{
    /// <summary>
    /// Request scene be started
    /// </summary>
    /// <summary>
    /// how the scene will displayed to user
    /// </summary>
    public interface IScene
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> TriggerSceneStart;

        void NameOfScene(SceneStringInputSigDelegate callback);

    }

    public delegate void SceneBoolInputSigDelegate(BoolInputSig boolInputSig, IScene scene);
    public delegate void SceneStringInputSigDelegate(StringInputSig stringInputSig, IScene scene);

    /// <summary>
    /// Scene can set multiple lights to distinct levels
    /// </summary>
    internal class Scene : IScene, IDisposable
    {
        #region Standard CH5 Component members

        private ComponentMediator ComponentMediator { get; set; }

        public object UserObject { get; set; }

        public uint ControlJoinId { get; private set; }

        private IList<BasicTriListWithSmartObject> _devices;
        public IList<BasicTriListWithSmartObject> Devices { get { return _devices; } }

        #endregion

        #region Joins

        private static class Joins
        {
            internal static class Booleans
            {
                public const uint TriggerSceneStart = 1;

            }
            internal static class Strings
            {

                public const uint NameOfScene = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Scene(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.TriggerSceneStart, onTriggerSceneStart);

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

        public event EventHandler<UIEventArgs> TriggerSceneStart;
        private void onTriggerSceneStart(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = TriggerSceneStart;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }



        public void NameOfScene(SceneStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.NameOfScene], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "Scene", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            TriggerSceneStart = null;
        }

        #endregion

    }
}
