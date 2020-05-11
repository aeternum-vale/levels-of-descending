using UnityEngine;

namespace InventoryModule
{
    public class InventoryItemData
    {
        public bool IsDisposable;
        public bool IsInStock;
        public GameObject Container;
        public GameObject Content;
        public Animator AnimatorComponent;
    }
}