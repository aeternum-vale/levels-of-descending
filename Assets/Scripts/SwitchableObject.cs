using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SwitchableObject : SelectableObject
{
 
    [SerializeField] ESwitchableObjectID id;
    [SerializeField] protected bool isDisposable;
    [SerializeField] EInventoryItemID necessaryInventoryItem;
    [SerializeField] bool hasValueOfNecessaryInventoryItem;

    Animator anim;
    public bool IsOpened { get; set; }
    public bool IsSealed { get; set; }
    bool isAnimationOn;

    static readonly string directionParamName = "Direction";
    static readonly string switchStateName = "Switch";

    protected string animationStateName = switchStateName;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnClick(EInventoryItemID? selectedInventoryItemId = null)
    {
        base.OnClick(selectedInventoryItemId);

        if (IsSealed)
        {
            return;
        }

        if (IsOpened || SwitchCondition(selectedInventoryItemId))
        {
            Switch();
        }
    }

    public virtual void Switch()
    {
        if (!isAnimationOn)
        {
            if (!IsOpened)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
    }

    protected virtual void Open()
    {
        anim.SetFloat(directionParamName, 1f);
        anim.Play(animationStateName, -1, 0f);

        IsOpened = true;
        Messenger<ESwitchableObjectID>.Broadcast(Events.SWITCHABLE_OBJECT_WAS_OPENED, id);

        if (isDisposable)
        {
            IsSealed = true;
            IsGlowingEnabled = false;
        }
    }

    protected virtual void Close()
    {
        IsOpened = false;
        anim.SetFloat(directionParamName, -1f);
        anim.Play(animationStateName, -1, 1f);
    }

    protected virtual bool SwitchCondition(EInventoryItemID? selectedInventoryItemId = null)
    {
        if (hasValueOfNecessaryInventoryItem)
        {
            return (selectedInventoryItemId == necessaryInventoryItem);
        } else
        {
            return true;
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