using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SwitchableObject : MultiStateObject
{
    [SerializeField] ESwitchableObjectID id;
    [SerializeField] EInventoryItemID necessaryInventoryItem;
    [SerializeField] bool hasValueOfNecessaryInventoryItem;

    public bool IsOpened { get; set; }

    static readonly string directionParamName = "Direction";
    static readonly string switchStateName = "Switch";

    protected override void Awake()
    {
        base.Awake();
        stateCount = 2;

        if (hasValueOfNecessaryInventoryItem)
        {
            anticipatedInventoryItemDict.Add(1, necessaryInventoryItem);
        }

        stateNameDict.Add(1, switchStateName);
        stateNameDict.Add(0, switchStateName);
    }

    protected override void ApplyState(byte state)
    {

        if (state == 1)
        {
            if (!OpenCondition()) return;

            OnOpen();
        }
        else
        {
            OnClose();
        }

        base.ApplyState(state);
    }


    public virtual void Switch()
    {
        if (currentState == 0)
        {
            ApplyState(1);
        } else
        {
            ApplyState(0);
        }
    }

    protected virtual void OnOpen()
    {
        anim.SetFloat(directionParamName, 1f);
        IsOpened = true;
        Messenger<ESwitchableObjectID>.Broadcast(Events.SWITCHABLE_OBJECT_WAS_OPENED, id);
    }

    protected virtual void OnClose()
    {
        IsOpened = false;
        anim.SetFloat(directionParamName, -1f);
    }

    protected virtual bool OpenCondition()
    {
        return true;
    }

    protected override void PlayAnimation(string name)
    {
        if (currentState == 0)
        {
            anim.Play(name, -1, 1f);
        } else
        {
            base.PlayAnimation(name);
        }
    }

}