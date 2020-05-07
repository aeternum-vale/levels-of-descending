using System;
using UnityEngine;

namespace SelectableObjectsModule.Utilities
{
    public class SelectableObjectClickedEventArgs : EventArgs
    {
        public readonly EInventoryItemId? SelectedInventoryItemId;
        public readonly GameObject ColliderCarrier;

        public SelectableObjectClickedEventArgs(EInventoryItemId? selectedInventoryItemId, GameObject colliderCarrier)
        {
            SelectedInventoryItemId = selectedInventoryItemId;
            ColliderCarrier = colliderCarrier;
        }
    }
}