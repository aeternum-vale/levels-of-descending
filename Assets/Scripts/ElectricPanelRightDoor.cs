﻿using System.Linq.Expressions;
using UnityEngine;

public class ElectricPanelRightDoor : SwitchableObject
{
    protected override bool SwitchCondition(EInventoryItemID? selectedInventoryItemId = null) =>
        (selectedInventoryItemId == EInventoryItemID.E_PANEL_KEY);
}
