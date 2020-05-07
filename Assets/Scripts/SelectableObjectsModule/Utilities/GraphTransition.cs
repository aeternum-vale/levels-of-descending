using System;

namespace SelectableObjectsModule.Utilities
{
    public class GraphTransition
    {
        public Func<bool> Condition;
        public bool IsReverse;
        public byte NextStateId;
        public EInventoryItemId? SelectedInventoryItemId;
    }
}