using UnityEngine;

public class InventoryObject : SelectableObject
{
    [SerializeField] protected EInventoryItemID objectId;

    public override void OnClick(EInventoryItemID? selectedInventoryObjectId = null)
    {
        if (IsGlowingEnabled)
        {
            GetComponent<Renderer>().enabled = false;
            Messenger<EInventoryItemID>.Broadcast(Events.INVENTORY_ITEM_WAS_CLICKED, objectId);
        }
    }
}