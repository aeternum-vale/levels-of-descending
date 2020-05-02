using System.Collections.Generic;
using UnityEngine;

public class MultiStateObject : SelectableObject
{
    protected byte currentState;
    protected byte stateCount;

    [SerializeField] protected bool isDisposable;

    public bool IsSealed { get; set; }
    protected string animationStateName;
    Animator anim;
    protected bool isAnimationOn;

    protected Dictionary<byte, EInventoryItemID> anticipatedInventoryItemDict = new Dictionary<byte, EInventoryItemID>();
    protected Dictionary<byte, string> stateNameDict = new Dictionary<byte, string>();

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    protected virtual void OnAnimationEnd()
    {
        isAnimationOn = !isAnimationOn;
    }

    protected virtual void OnAnimationStart()
    {
        isAnimationOn = !isAnimationOn;
    }
    public override void OnClick(EInventoryItemID? selectedInventoryItemId = null)
    {
        base.OnClick(selectedInventoryItemId);

        if (IsSealed) return;
        if (isAnimationOn) return;

        if (!anticipatedInventoryItemDict.ContainsKey(currentState) ||
            (anticipatedInventoryItemDict[currentState] == selectedInventoryItemId))
        {
            ApplyState((byte)(currentState + 1 % stateCount));
        }
    }

    protected virtual void ApplyState(byte state)
    {
        if ((state == stateCount - 1) && (isDisposable))
        {
            IsSealed = true;
            IsGlowingEnabled = false;
        }

        currentState = state;
        anim.Play(stateNameDict[state], -1, 0f);
    }
}
