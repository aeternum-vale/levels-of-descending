using UnityEngine;

public class InventoryObject : SelectableObject
{
    [SerializeField] protected EInventoryItemID objectId;
    [SerializeField] protected GameObject mesh;

    public override void OnClick(EInventoryItemID? selectedInventoryObjectId = null)
    {
        GetComponent<Renderer>().enabled = false;
        Messenger<EInventoryItemID>.Broadcast(Events.ADD_ITEM_TO_INVENTORY, objectId);
    }
}