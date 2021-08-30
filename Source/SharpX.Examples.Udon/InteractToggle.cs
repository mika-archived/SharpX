using SharpX.Library.Udon;
using SharpX.Library.Udon.Attributes;
using SharpX.Library.Udon.Enums;

using UnityEngine;

namespace SharpX.Examples.Udon
{
    [AddComponentMenu("Udon Sharp/Utilities/Interact Toggle")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class InteractToggle : SharpXUdonBehaviour
    {
        [Tooltip("List of objects to toggle on and off")]
        public GameObject[] toggleObjects;

        public override void Interact()
        {
            foreach (var toggleObject in toggleObjects)
                toggleObject.SetActive(!toggleObject.activeSelf);
        }
    }
}