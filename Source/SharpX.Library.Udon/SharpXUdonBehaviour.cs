using UnityEngine;

using VRC.SDK3.Components.Video;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon.Common.Enums;
using VRC.Udon.Common.Interfaces;

namespace SharpX.Library.Udon
{
    public class SharpXUdonBehaviour : MonoBehaviour
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
        public object GetProgramVariable(string name)
        {
            return default;
        }

        public void SetProgramVariable(string name, object value) { }

        public void SendCustomEvent(string eventName) { }

        public void SendCustomNetworkEvent(NetworkEventTarget target, string eventName) { }

        public void SendCustomEventDelayedSeconds(string eventName, float delaySeconds, EventTiming eventTiming = EventTiming.Update) { }

        public void SendCustomEventDelayedFrames(string eventName, int delayFrames, EventTiming eventTiming = EventTiming.Update) { }

        public bool DisableInteractive { get; set; }

        public static GameObject VRCInstantiate(GameObject original)
        {
            return Instantiate(original);
        }

        public void RequestSerialization() { }

        public virtual void PostLateUpdate() { }
        public virtual void Interact() { }
        public virtual void OnDrop() { }
        public virtual void OnOwnershipTransferred(VRCPlayerApi player) { }
        public virtual void OnPickup() { }
        public virtual void OnPickupUseDown() { }
        public virtual void OnPickupUseUp() { }
        public virtual void OnPlayerJoined(VRCPlayerApi player) { }
        public virtual void OnPlayerLeft(VRCPlayerApi player) { }
        public virtual void OnSpawn() { }
        public virtual void OnStationEntered(VRCPlayerApi player) { }
        public virtual void OnStationExited(VRCPlayerApi player) { }
        public virtual void OnVideoEnd() { }
        public virtual void OnVideoError(VideoError videoError) { }
        public virtual void OnVideoLoop() { }
        public virtual void OnVideoPause() { }
        public virtual void OnVideoPlay() { }
        public virtual void OnVideoReady() { }
        public virtual void OnVideoStart() { }
        public virtual void OnPreSerialization() { }
        public virtual void OnDeserialization() { }
        public virtual void OnPlayerTriggerEnter(VRCPlayerApi player) { }
        public virtual void OnPlayerTriggerExit(VRCPlayerApi player) { }
        public virtual void OnPlayerTriggerStay(VRCPlayerApi player) { }
        public virtual void OnPlayerCollisionEnter(VRCPlayerApi player) { }
        public virtual void OnPlayerCollisionExit(VRCPlayerApi player) { }
        public virtual void OnPlayerCollisionStay(VRCPlayerApi player) { }
        public virtual void OnPlayerParticleCollision(VRCPlayerApi player) { }
        public virtual void OnPlayerRespawn(VRCPlayerApi player) { }

        public virtual void OnPostSerialization(SerializationResult result) { }

        public virtual bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            return true;
        }

        public virtual void MidiNoteOn(int channel, int number, int velocity) { }
        public virtual void MidiNoteOff(int channel, int number, int velocity) { }
        public virtual void MidiControlChange(int channel, int number, int value) { }

        public virtual void InputJump(bool value, UdonInputEventArgs args) { }
        public virtual void InputUse(bool value, UdonInputEventArgs args) { }
        public virtual void InputGrab(bool value, UdonInputEventArgs args) { }
        public virtual void InputDrop(bool value, UdonInputEventArgs args) { }
        public virtual void InputMoveHorizontal(float value, UdonInputEventArgs args) { }
        public virtual void InputMoveVertical(float value, UdonInputEventArgs args) { }
        public virtual void InputLookHorizontal(float value, UdonInputEventArgs args) { }
        public virtual void InputLookVertical(float value, UdonInputEventArgs args) { }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        SerializationData serializationData;

        void OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref serializationData);
        }

        void OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData);
        }
        
        [OdinSerialize]
        private IUdonBehaviour _backingUdonBehaviour = null;
#endif
    }
}