using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class SwitchableObject : MultiStateObject
{
    [SerializeField] ESwitchableObjectID id;
    [SerializeField] EInventoryItemID necessaryInventoryItem;
    [SerializeField] bool hasValueOfNecessaryInventoryItem;

    public bool IsOpened { get; set; }

    static readonly string switchStateName = "Switch";

    protected override void Awake()
    {
        base.Awake();

        states = new List<GraphState>()
        {
            new GraphState() { name = switchStateName, onReached = this.OnClose }, //0 - close
            new GraphState() { name = switchStateName, onReached = this.OnOpen }   //1 - open
        };

        stateTransitions = new Dictionary<byte, List<GraphTransition>>()
        {
            { (byte) ESwitchableObjectStateId.CLOSE, new List<GraphTransition> ()
                {
                    new GraphTransition() {
                        nextStateId = (byte) ESwitchableObjectStateId.OPEN,
                        selectedInventoryItemId = (hasValueOfNecessaryInventoryItem ? necessaryInventoryItem : (EInventoryItemID?)null),
                        condition = this.OpenCondition,
                    }
                }
            },

            { (byte) ESwitchableObjectStateId.OPEN, new List<GraphTransition>()
                {
                    new GraphTransition() {
                        nextStateId = (byte) ESwitchableObjectStateId.CLOSE,
                        isReverse = true
                    }
                }
            },
        };
    }

    public virtual void Switch()
    {
        MakeTransition(stateTransitions[currentStateId][0]);
    }

    protected virtual void OnOpen()
    {
        IsOpened = true;
        Messenger<ESwitchableObjectID>.Broadcast(Events.SWITCHABLE_OBJECT_WAS_OPENED, id);

        if (isDisposable)
        {
            Seal();
        }
    }

    protected virtual void OnClose()
    {
        IsOpened = false;
    }

    protected virtual bool OpenCondition()
    {
        return true;
    }

    public static string GetPath(ESwitchableObjectID id)
    {
        return GameConstants.switchableObjectToInstancePathMap[id];

    }
    public static string GetName(ESwitchableObjectID id)
    {
        return GameUtils.GetNameByPath(GameConstants.switchableObjectToInstancePathMap[id]);
    }
}