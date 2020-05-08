using System;
using UnityEngine;

namespace SelectableObjectsModule
{
    [RequireComponent(typeof(Animator))]
    public class SwitchableObject : SelectableObject
    {
        private static readonly int DirectionParamHash = Animator.StringToHash("Direction");
        private static readonly int DefaultStateNameHash = Animator.StringToHash("Switch");
        private Animator _animator;
        protected bool IsAnimationOn;
        [SerializeField] private ESwitchableObjectId id;
        [SerializeField] protected bool isDisposable;
        [SerializeField] private EInventoryItemId necessaryInventoryItem;
        [SerializeField] private bool hasValueOfNecessaryInventoryItem;

        public bool IsOpened { get; protected set; }
        public bool IsSealed { get; set; }
        public bool PreventSwitching { get; set; }

        public EInventoryItemId? NecessaryInventoryItem { get; set; }

        public Func<bool> OpenCondition { get; set; }

        public int AnimationNameHash { get; set; }

        public event EventHandler Opened;
        public event EventHandler Closed;
        public event EventHandler OpenAnimationCompleted;
        public event EventHandler CloseAnimationCompleted;

        protected virtual void Start()
        {
            _animator = GetComponent<Animator>();
        }

        protected override void Awake()
        {
            base.Awake();
            if (hasValueOfNecessaryInventoryItem)
                NecessaryInventoryItem = necessaryInventoryItem;

            AnimationNameHash = DefaultStateNameHash;
        }

        public override void OnClick(EInventoryItemId? selectedInventoryItemId, GameObject colliderCarrier)
        {
            base.OnClick(selectedInventoryItemId, colliderCarrier);

            if (!PreventSwitching)
                Switch(selectedInventoryItemId);

            PreventSwitching = false;
        }

        public virtual void Switch(EInventoryItemId? selectedInventoryItemId = null)
        {
            if (IsAnimationOn) return;
            if (IsSealed) return;

            if (IsOpened)
            {
                if (selectedInventoryItemId == null)
                    Close();
            }
            else
            {
                if ((OpenCondition == null || OpenCondition()) && NecessaryInventoryItem == selectedInventoryItemId)
                    Open();
            }
        }

        public virtual void Open()
        {
            IsOpened = true;
            if (isDisposable)
                SealAndDisableGlowing();

            Opened?.Invoke(this, EventArgs.Empty);

            PlayAnimation();
        }

        public virtual void Close()
        {
            IsOpened = false;

            Closed?.Invoke(this, EventArgs.Empty);

            PlayAnimation(true);
        }

        public virtual void SealAndDisableGlowing()
        {
            IsSealed = true;
            IsGlowingEnabled = false;
        }

        protected virtual void PlayAnimation(bool isReverse = false)
        {
            if (isReverse)
            {
                _animator.SetFloat(DirectionParamHash, -1f);
                _animator.Play(AnimationNameHash, -1, 1f);
            }
            else
            {
                _animator.SetFloat(DirectionParamHash, 1f);
                _animator.Play(AnimationNameHash, -1, 0f);
            }
        }

        protected virtual void OnAnimationEnd()
        {
            IsAnimationOn = !IsAnimationOn;

            if (IsOpened)
                OpenAnimationCompleted?.Invoke(this, EventArgs.Empty);
            else
                CloseAnimationCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAnimationStart()
        {
            IsAnimationOn = !IsAnimationOn;
        }
    }
}