using SharpX.Library.Udon;
using SharpX.Library.Udon.Attributes;

using UnityEngine;

using VRC.SDKBase;

namespace SharpX.Examples.Udon
{
    public class MasterToggleObject : SharpXUdonBehaviour
    {
        [UdonSynced]
        private bool isObjectEnabled;

        [SerializeField]
        private GameObject toggleObject;

        private void Start()
        {
            isObjectEnabled = toggleObject.activeSelf;
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            return requestedOwner.isMaster;
        }

        public override void OnDeserialization()
        {
            toggleObject.SetActive(isObjectEnabled);
        }

        public override void Interact()
        {
            if (!Networking.IsMaster)
                return;
            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);

            isObjectEnabled = !isObjectEnabled;
            toggleObject.SetActive(isObjectEnabled);

            RequestSerialization();
        }
    }
}