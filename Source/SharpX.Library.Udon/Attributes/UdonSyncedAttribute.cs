using System;

using SharpX.Library.Udon.Enums;

namespace SharpX.Library.Udon.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UdonSyncedAttribute : Attribute
    {
        public UdonSyncMode SyncMode { get; }

        public UdonSyncedAttribute(UdonSyncMode mode = UdonSyncMode.None)
        {
            SyncMode = mode;
        }
    }
}