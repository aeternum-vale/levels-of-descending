using Plugins;
using SelectableObjectsModule;
using UnityEngine;

namespace InventoryModule
{
    public class InventoryObject : SelectableObject
    {
        [SerializeField] protected bool isGrabable = true;
        [SerializeField] protected EInventoryItemId objectId;

        public bool IsGrabable
        {
            get => isGrabable;
            set => isGrabable = value;
        }

        public override void OnClick(EInventoryItemId? selectedInventoryObjectId, GameObject colliderCarrier)
        {
            if (!isGrabable) return;

            gameObject.SetActive(false);
            Messenger<EInventoryItemId>.Broadcast(Events.InventoryItemWasClicked, objectId);
        }

        public override void OnOver(GameObject colliderCarrier)
        {
            if (!isGrabable) return;

            base.OnOver(colliderCarrier);
        }
    }
}