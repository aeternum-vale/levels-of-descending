public class PostboxDoor : SwitchableObject
{
    protected override bool SwitchCondition(EInventoryItemID? selectedInventoryItemId = null)
    {
        return (selectedInventoryItemId == EInventoryItemID.POSTBOX_KEY);
    }
}
