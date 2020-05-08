using System.Collections.Generic;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule
{
    public class PushableObject : SwitchableObject
    {
        private static readonly int PushStateNameHash = Animator.StringToHash("Push");
        protected override void Awake()
        {
            base.Awake();
            AnimationNameHash = PushStateNameHash;
        }
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