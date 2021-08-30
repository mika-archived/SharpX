using SharpX.Library.Udon;

using UnityEngine;

using VRC.SDKBase;

namespace SharpX.Examples.Udon
{
    [AddComponentMenu("SharpX.Udon/Utilities/Bone Follower")]
    public class BoneFollower : SharpXUdonBehaviour
    {
        private bool isInEditor;

        private VRCPlayerApi playerApi;

        [SerializeField]
        private HumanBodyBones trackedBone;

        private void Start()
        {
            playerApi = Networking.LocalPlayer;
            isInEditor = playerApi == null;
        }

        private void Update()
        {
            if (isInEditor)
                return;

            transform.SetPositionAndRotation(playerApi.GetBonePosition(trackedBone), playerApi.GetBoneRotation(trackedBone));
        }
    }
}