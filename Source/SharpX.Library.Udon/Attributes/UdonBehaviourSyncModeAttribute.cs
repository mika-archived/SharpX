using System;

using SharpX.Library.Udon.Enums;

namespace SharpX.Library.Udon.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UdonBehaviourSyncModeAttribute : Attribute
    {
        public BehaviourSyncMode SyncMode { get; }

        public UdonBehaviourSyncModeAttribute(BehaviourSyncMode syncMode = BehaviourSyncMode.Any)
        {
            SyncMode = syncMode;
        }
    }
}