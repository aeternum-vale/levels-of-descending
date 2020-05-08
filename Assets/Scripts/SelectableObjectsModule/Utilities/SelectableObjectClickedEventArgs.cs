using System;
using UnityEngine;

namespace SelectableObjectsModule.Utilities
{
    public class SelectableObjectClickedEventArgs : EventArgs
    {
        public readonly GameObject ColliderCarrier;
        public readonly EInventoryItemId? SelectedInventoryItemId;

        public SelectableObjectClickedEventArgs(EInventoryItemId? selectedInventoryItemId, GameObject colliderCarrier)
        {
            SelectedInventoryItemId = selectedInventoryItemId;
            ColliderCarrier = colliderCarrier;
        }
    }
}