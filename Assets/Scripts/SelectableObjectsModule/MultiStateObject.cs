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
        protected bool IsAnimationOn;

        protected List<GraphState> States;
        protected Dictionary<byte, List<GraphTransition>> StateTransitions;

        private const string DirectionParamName = "Direction";

        public event EventHandler<MultiStateObjectEventArgs> OnStateReached;

        protected virtual void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public override void OnClick(EInventoryItemId? selectedInventoryItemId, GameObject colliderCarrier)
        {
            base.OnClick(selectedInventoryItemId, colliderCarrier);

            if (IsSealed) return;
            if (IsAnimationOn) return;

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

        protected virtual void PlayAnimation(string name, bool isReverse)
        {
            if (isReverse)
            {
                _animator.SetFloat(DirectionParamName, -1f);
                _animator.Play(name, -1, 1f);
            }
            else
            {
                _animator.SetFloat(DirectionParamName, 1f);
                _animator.Play(name, -1, 0f);
            }
        }

        protected virtual void OnAnimationEnd()
        {
            IsAnimationOn = !IsAnimationOn;
        }

        protected virtual void OnAnimationStart()
        {
            IsAnimationOn = !IsAnimationOn;
        }
    }
}