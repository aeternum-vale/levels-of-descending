using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public abstract partial class MultiStateObject : SelectableObject
{
    [SerializeField] protected bool isDisposable;

    protected Animator anim;

    public bool IsSealed { get; set; }

    protected byte currentStateId;
    protected bool isAnimationOn;

    protected List<GraphState> states;
    protected Dictionary<byte, List<GraphTransition>> stateTransitions;

    static readonly string directionParamName = "Direction";

    public event EventHandler<MultiStateObjectEventArgs> OnStateReached;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnClick(EInventoryItemID? selectedInventoryItemId, GameObject colliderCarrier)
    {
        base.OnClick(selectedInventoryItemId, colliderCarrier);

        if (IsSealed) return;
        if (isAnimationOn) return;

        foreach (var transition in stateTransitions[currentStateId])
        {
            if ((transition.condition == null || transition.condition())
                && (transition.selectedInventoryItemId == selectedInventoryItemId))
            {
                MakeTransition(transition);
                return;
            }
        }
    }

    protected virtual void MakeTransition(GraphTransition transition)
    {
        if (stateTransitions.ContainsKey(transition.nextStateId) && isDisposable)
        {
            Seal();
        }

        currentStateId = transition.nextStateId;
        states[currentStateId].onReached?.Invoke();
        OnStateReached?.Invoke(this, new MultiStateObjectEventArgs(currentStateId));
        PlayAnimation(states[transition.nextStateId].name, transition.isReverse);
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
            anim.SetFloat(directionParamName, -1f);
            anim.Play(name, -1, 1f);
        }
        else
        {
            anim.SetFloat(directionParamName, 1f);
            anim.Play(name, -1, 0f);
        }

    }
    protected virtual void OnAnimationEnd()
    {
        isAnimationOn = !isAnimationOn;
    }

    protected virtual void OnAnimationStart()
    {
        isAnimationOn = !isAnimationOn;
    }
}
