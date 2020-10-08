using System;
using Plugins;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule
{
    [RequireComponent(typeof(Animator))]
    public class SwitchableObject : SelectableObject, IInitStateReturnable
    {
        public static readonly int defaultSwitchStateNameHash = Animator.StringToHash("Switch");
        private static readonly int DirectionParamHash = Animator.StringToHash("Direction");

        private Animator _animator;
        private AudioSource _audioSource;
        
        [SerializeField] private ESwitchableObjectId id;
        [SerializeField] private EInventoryItemId necessaryInventoryItem;
        [SerializeField] private bool hasValueOfNecessaryInventoryItem;
        [SerializeField] private int initStateSafeDistanceToPlayer = 1;
        [SerializeField] private bool isDependent;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        [SerializeField] protected bool isDisposable;
        
        protected bool IsAnimationOn;
        public bool IsOpened { get; private set; }
        public bool IsDependent { get; private set; }

        public bool IsSealed { get; set; }
        public bool PreventSwitching { get; set; }

        public EInventoryItemId? NecessaryInventoryItem { get; set; }

        public Func<bool> OpenCondition { get; set; }

        public int AnimationNameHash { get; set; }

        public int InitStateSafeDistanceToPlayer { get; set; } = 1;

        public void ReturnToInitState(int floorDistanceToPlayer)
        {
            if (IsDependent) return;
            if (floorDistanceToPlayer < InitStateSafeDistanceToPlayer) return;

            Close(true);
            IsSealed = false;
            IsGlowingEnabled = true;
        }

        public event EventHandler Opened;
        public event EventHandler Closed;
        public event EventHandler OpenAnimationCompleted;
        public event EventHandler CloseAnimationCompleted;

        protected virtual void Start()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        protected override void Awake()
        {
            base.Awake();

            if (hasValueOfNecessaryInventoryItem)
                NecessaryInventoryItem = necessaryInventoryItem;

            InitStateSafeDistanceToPlayer = initStateSafeDistanceToPlayer;

            AnimationNameHash = defaultSwitchStateNameHash;
            IsDependent = isDependent;
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
                else
                    Messenger.Broadcast(Events.InventoryItemUsedIncorrectly);
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
                else
                {
                    if (selectedInventoryItemId != null)
                        Messenger.Broadcast(Events.InventoryItemUsedIncorrectly);
                }
            }
        }

        public virtual void Open(bool immediately = false)
        {
            IsOpened = true;
            if (isDisposable)
                SealAndDisableGlowing();

            PlayAnimation(immediately);

            if (!immediately)
                PlayOpenSound();

            Opened?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Close(bool immediately = false)
        {
            IsOpened = false;

            PlayAnimation(immediately);

            if (!immediately)
                PlayCloseSound();

            Closed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SealAndDisableGlowing()
        {
            IsSealed = true;
            IsGlowingEnabled = false;
        }

        protected virtual void PlayAnimation(bool immediately = false)
        {
            if (_animator == null) return;

            if (!IsOpened)
            {
                if (immediately)
                {
                    _animator.Play(GameConstants.idleStateNameHash, -1, 1f);
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

        private void PlayOpenSound()
        {
            PlaySound(openSound);
        }

        private void PlayCloseSound()
        {
            PlaySound(closeSound);
        }

        private void PlaySound(AudioClip clip)
        {
            if (_audioSource == null) return;
            if (clip == null) return;

            _audioSource.PlayOneShot(clip);
        }
    }
}