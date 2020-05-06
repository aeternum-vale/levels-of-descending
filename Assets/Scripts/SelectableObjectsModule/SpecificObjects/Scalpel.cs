﻿using InventoryModule;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class Scalpel : InventoryObject
    {
        private Animator _anim;
        private static readonly int Active = Animator.StringToHash("Active");

        protected override void Awake()
        {
            base.Awake();
            IsGlowingEnabled = false;
            _anim = GetComponent<Animator>();
        }

        public void Emerge()
        {
            gameObject.SetActive(true);
            _anim.SetTrigger(Active);
        }

        private void OnEmergeAnimationEnd()
        {
            IsGlowingEnabled = true;
        }
    }
}