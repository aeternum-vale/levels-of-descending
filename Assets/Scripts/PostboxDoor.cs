using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PostboxDoor : SwitchableSelectableObject
{
    protected override bool SwitchCondition(EInventoryItemID? selectedInventoryItemId = null) =>
        (selectedInventoryItemId == EInventoryItemID.POSTBOX_KEY);
}
