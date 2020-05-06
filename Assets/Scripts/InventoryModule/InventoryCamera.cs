using System;
using UnityEngine;

namespace InventoryModule
{
    public class InventoryCamera : MonoBehaviour
    {
        public Action DrawInventory { get; set; }
        public bool IsInventoryModeOn { get; set; }

        private void OnGUI()
        {
            if (IsInventoryModeOn) DrawInventory.Invoke();
        }
    }
}