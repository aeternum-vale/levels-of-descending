using System;
using Plugins;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule
{
    [RequireComponent(typeof(Animator))]
    public class SwitchableObject : SelectableObject, IInitStateReturnable
    {
        private static readonly int DirectionParamHash = Animator.StringToHash("Direction");
        private static readonly int DefaultSwitchStateNameHash = Animator.StringToHash("Switch");
        private static readonly int IdleStateNameHash = Animator.StringToHash("Idle");

        private Animator _animator;

        [SerializeField] private ESwitchableObjectId id;
        protected bool IsAnimationOn;
        [SerializeField] protected bool isDisposable;
        [SerializeField] private EInventoryItemId necessaryInventoryItem;
        [SerializeField] private bool hasValueOfNecessaryInventoryItem;

        public bool IsOpened { get; protected set; }
        public bool IsSealed { get; set; }
        public bool PreventSwitching { get; set; }

        public EInventoryItemId? NecessaryInventoryItem { get; set; }

        public Func<bool> OpenCondition { get; set; }

        public int AnimationNameHash { get; set; }

        public void ReturnToInitState()
        {
            if (!gameObject.activeSelf) return;

            Close(true);
        }

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

            AnimationNameHash = DefaultSwitchStateNameHash;
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
                {
                    if (selectedInventoryItemId != null)
                        Messenger<EInventoryItemId>.Broadcast(Events.InventoryItemWasSuccessfullyUsed,
                            (EInventoryItemId) selectedInventoryItemId);

                    Open();
                }
            }
        }

        public virtual void Open(bool immediately = false)
        {
            IsOpened = true;
            if (isDisposable)
                SealAndDisableGlowing();

            PlayAnimation(immediately);

            Opened?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Close(bool immediately = false)
        {
            IsOpened = false;
            PlayAnimation(immediately);

            Closed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SealAndDisableGlowing()
        {
            IsSealed = true;
            IsGlowingEnabled = false;
        }

        protected virtual void PlayAnimation(bool immediately = false)
        {
            if (!IsOpened)
            {
                if (immediately)
                {
                    _animator.Play(IdleStateNameHash, -1, 1f);
                }
                else
                {
                    _animator.SetFloat(DirectionParamHash, -1f);
                    _animator.Play(AnimationNameHash, -1, 1f);
                }
            }
            else
            {
                _animator.SetFloat(DirectionParamHash, 1f);
                _animator.Play(AnimationNameHash, -1, immediately ? 1f : 0f);
            }
        }

        protected virtual void OnAnimationStart()
        {
            if (IsOpened)
            {
                IsAnimationOn = true;
            }
            else
            {
                IsAnimationOn = false;
                CloseAnimationCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void OnAnimationEnd()
        {
            if (!IsOpened)
            {
                IsAnimationOn = true;
            }
            else
            {
                IsAnimationOn = false;
                OpenAnimationCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}