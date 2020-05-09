using Plugins;
using UnityEngine;

namespace SelectableObjectsModule
{
    public class InventoryObject : SelectableObject
    {
        [SerializeField] protected bool isGrabable = true;
        [SerializeField] protected EInventoryItemId objectId;

        public bool IsGrabable { get; set; }

        protected override void Awake()
        {
            base.Awake();
            IsGrabable = isGrabable;
        }

        public override void OnClick(EInventoryItemId? selectedInventoryObjectId, GameObject colliderCarrier)
        {
            if (!IsGrabable) return;

            gameObject.SetActive(false);
            Messenger<EInventoryItemId>.Broadcast(Events.InventoryItemWasClicked, objectId);
        }

        public override void OnOver(GameObject colliderCarrier)
        {
            if (!IsGrabable) return;

            base.OnOver(colliderCarrier);
        }
    }
}