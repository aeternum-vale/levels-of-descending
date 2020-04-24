public class ElectricPanelRightDoor : SwitchableObject
{
    protected override bool SwitchCondition(EInventoryItemID? selectedInventoryItemId = null)
    {
        return (selectedInventoryItemId == EInventoryItemID.E_PANEL_KEY);
    }
}
