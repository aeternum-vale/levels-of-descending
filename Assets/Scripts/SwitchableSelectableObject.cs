using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SwitchableSelectableObject : SelectableObject
{

    [SerializeField] ESwitchableObjectID id;
    Animator anim;
    public bool IsOpened { get; set; }
    bool isAnimationOn;

    readonly string switchStateName = "Switch";
    readonly string directionParamName = "Direction";

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnClick(EInventoryItemID? selectedInventoryItemId = null)
    {
        base.OnClick(selectedInventoryItemId);
        Messenger<ESwitchableObjectID>.Broadcast(Events.SWITCHABLE_OBJECT_OPENED, id);
        Switch();
    }

    public void Switch()
    {
        if (!isAnimationOn)
        {
            if (!IsOpened)
            {
                IsOpened = true;
                anim.SetFloat(directionParamName, 1f);
                anim.Play(switchStateName, -1, 0f);
            }
            else
            {
                IsOpened = false;
                anim.SetFloat(directionParamName, -1f);
                anim.Play(switchStateName, -1, 1f);
            }
        }
    }

    void OnAnimationEnd()
    {
        isAnimationOn = !isAnimationOn;
    }

    void OnAnimationStart()
    {
        isAnimationOn = !isAnimationOn;
    }
}