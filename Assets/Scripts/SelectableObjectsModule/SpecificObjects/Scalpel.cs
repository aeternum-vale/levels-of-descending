using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class Scalpel : InventoryObject
    {
        private static readonly int Active = Animator.StringToHash("Active");
        private Animator _anim;

        protected override void Awake()
        {
            base.Awake();
            IsGlowingEnabled = false;
            _anim = GetComponent<Animator>();
        }

        public void Emerge()
        {
            _anim.SetTrigger(Active);
        }

        private void OnEmergeAnimationEnd()
        {
            IsGlowingEnabled = true;
        }
    }
}