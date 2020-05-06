using System;
using System.Collections.Generic;
using UnityEngine;

public class GarbageChuteDoor : SwitchableObject
{
    bool isUnhinged;
    static readonly string removingStateName = "Removing";
    GameObject rigidDoor;
    byte removingStateId = 2;

    protected override void Start()
    {
        base.Start();
        rigidDoor = transform.parent.Find("rigid_garbage_chute_door").gameObject;
    }

    protected override void Awake()
    {
        base.Awake();

        states.Add(new GraphState() { name = removingStateName, onReached = this.OnRemove });
        stateTransitions[(byte)ESwitchableObjectStateId.CLOSE][0].condition = () => !isUnhinged;

        stateTransitions[(byte)ESwitchableObjectStateId.CLOSE].Add(new GraphTransition() { nextStateId = removingStateId, condition = () => isUnhinged });
    }

    public void Unhinge()
    {
        isUnhinged = true;
    }

    void OnRemove()
    {
        if (isUnhinged)
        {
            IsGlowingEnabled = false;
        }
    }

    protected override void OnAnimationEnd()
    {
        base.OnAnimationEnd();

        if (isUnhinged)
        {
            rigidDoor.SetActive(true);
        }
    }
}
