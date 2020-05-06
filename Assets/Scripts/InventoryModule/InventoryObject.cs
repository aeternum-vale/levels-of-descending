using Plugins;
using SelectableObjectsModule;
using UnityEngine;

namespace InventoryModule
{
    public class InventoryObject : SelectableObject
    {
        [SerializeField] protected EInventoryItemId objectId;
        [SerializeField] protected bool isGrabable = true;

        public bool IsGrabable
        {
            get => isGrabable;
            set => isGrabable = value;
        }

        public override void OnClick(EInventoryItemId? selectedInventoryObjectId, GameObject colliderCarrier)
        {
            if (!isGrabable) return;
            
            gameObject.SetActive(false);
            Messenger<EInventoryItemId>.Broadcast(Events.inventoryItemWasClicked, objectId);
        }

        public override void OnOver(GameObject colliderCarrier)
        {
            if (!isGrabable) return;

            base.OnOver(colliderCarrier);
        }

        public static string GetPath(EInventoryItemId id)
        {
            return GameConstants.inventoryItemToInstancePathMap[id];
        }

        public static string GetName(EInventoryItemId id)
        {
            return GameUtils.GetNameByPath(GameConstants.inventoryItemToInstancePathMap[id]);
        }
    }
}