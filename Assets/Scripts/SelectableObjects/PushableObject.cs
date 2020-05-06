using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PushableObject : MultiStateObject
{
    static readonly string animationName = "push";

    protected override void Awake()
    {
        base.Awake();

        states = new List<GraphState>()
        {
            new GraphState() { name = animationName, onReached = this.OnPush }
        };

        stateTransitions = new Dictionary<byte, List<GraphTransition>>()
        {
            { 0, new List<GraphTransition> ()
                {
                    new GraphTransition() {
                        nextStateId = 0
                    }
                }
            },
        };
    }

    protected virtual void OnPush()
    {

    }
}