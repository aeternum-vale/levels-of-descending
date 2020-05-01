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

    public override void OnClick(EInventoryItemID? selectedInventoryObjectId = null)
    {
        if (isGrabable)
        {
            gameObject.SetActive(false);
            Messenger<EInventoryItemID>.Broadcast(Events.INVENTORY_ITEM_WAS_CLICKED, objectId);
        }
    }

    public override void OnOver()
    {
        if (!isGrabable)
        {
            return;
        }

        base.OnOver();
    }
}