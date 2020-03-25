﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCamera : MonoBehaviour
{
    public Action DrawInventory { get; set; }
    public bool IsInventoryModeOn { get; set; }

    private void OnGUI()
    {
        if (IsInventoryModeOn)
        {
            DrawInventory.Invoke();
        }
    }
}
