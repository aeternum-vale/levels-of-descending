using System.Collections.Generic;
using SelectableObjectsModule.Utilities;

namespace SelectableObjectsModule
{
    public class PushableObject : SwitchableObject
    {
        public override void Switch(EInventoryItemId? selectedInventoryItemId = null)
        {
            if (IsAnimationOn) return;
            if (IsSealed) return;
            
            if (OpenCondition == null || OpenCondition())
            {
                Open();
            }
        }
    }
}