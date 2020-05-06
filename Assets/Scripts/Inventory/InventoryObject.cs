using UnityEngine;

public class InventoryObject : SelectableObject
{
    [SerializeField] protected EInventoryItemID objectId;
    [SerializeField] protected bool isGrabable = true;

    public bool IsGrabable
    {
        get { return isGrabable; }
        set { isGrabable = value; }
    }

    public override void OnClick(EInventoryItemID? selectedInventoryObjectId, GameObject colliderCarrier)
    {
        if (isGrabable)
        {
            gameObject.SetActive(false);
            Messenger<EInventoryItemID>.Broadcast(Events.INVENTORY_ITEM_WAS_CLICKED, objectId);
        }
    }

    public override void OnOver(GameObject colliderCarrier)
    {
        if (!isGrabable)
        {
            return;
        }

        base.OnOver(colliderCarrier);
    }

    public static string GetPath(EInventoryItemID id)
    {
        return GameConstants.inventoryItemToInstancePathMap[id];
    }
    public static string GetName(EInventoryItemID id)
    {
        return GameUtils.GetNameByPath(GameConstants.inventoryItemToInstancePathMap[id]);
    }
}