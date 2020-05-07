using System;
using System.Collections.Generic;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule
{
    public abstract class MultiStateObject : SelectableObject
    {
        private static readonly int Direction = Animator.StringToHash("Direction");
        private Animator _animator;
        private bool _isAnimationOn;

        protected byte CurrentStateId;
        [SerializeField] protected bool isDisposable;

        [NonSerialized] public List<GraphState> States;
        [NonSerialized] public Dictionary<byte, List<GraphTransition>> StateTransitions;

        public bool IsSealed { get; set; }

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
            States[CurrentStateId].OnAnimationEnd?.Invoke();
        }

        protected virtual void OnAnimationStart()
        {
            _isAnimationOn = !_isAnimationOn;
        }
    }
}