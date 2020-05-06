using System;
using System.Collections.Generic;
using UnityEngine;

namespace SelectableObjectsModule
{
    public abstract class MultiStateObject : SelectableObject
    {
        [SerializeField] protected bool isDisposable;

        private Animator _animator;

        public bool IsSealed { get; set; }

        protected byte CurrentStateId;
        private bool _isAnimationOn;

        protected List<GraphState> States;
        protected Dictionary<byte, List<GraphTransition>> StateTransitions;
        private static readonly int Direction = Animator.StringToHash("Direction");
        public event EventHandler<MultiStateObjectEventArgs> OnStateReached;

        protected virtual void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public override void OnClick(EInventoryItemId? selectedInventoryItemId, GameObject colliderCarrier)
        {
            base.OnClick(selectedInventoryItemId, colliderCarrier);

            if (IsSealed) return;
            if (_isAnimationOn) return;

            foreach (var transition in StateTransitions[CurrentStateId])
                if ((transition.Condition == null || transition.Condition())
                    && transition.SelectedInventoryItemId == selectedInventoryItemId)
                {
                    MakeTransition(transition);
                    return;
                }
        }

        protected virtual void MakeTransition(GraphTransition transition)
        {
            if (StateTransitions.ContainsKey(transition.NextStateId) && isDisposable) Seal();

            CurrentStateId = transition.NextStateId;
            States[CurrentStateId].OnReached?.Invoke();
            OnStateReached?.Invoke(this, new MultiStateObjectEventArgs(CurrentStateId));
            PlayAnimation(States[transition.NextStateId].Name, transition.IsReverse);
        }

        protected virtual void Seal()
        {
            IsSealed = true;
            IsGlowingEnabled = false;
        }

        protected virtual void PlayAnimation(string animationName, bool isReverse)
        {
            if (isReverse)
            {
                _animator.SetFloat(Direction, -1f);
                _animator.Play(animationName, -1, 1f);
            }
            else
            {
                _animator.SetFloat(Direction, 1f);
                _animator.Play(animationName, -1, 0f);
            }
        }

        protected virtual void OnAnimationEnd()
        {
            _isAnimationOn = !_isAnimationOn;
        }

        protected virtual void OnAnimationStart()
        {
            _isAnimationOn = !_isAnimationOn;
        }
    }
}