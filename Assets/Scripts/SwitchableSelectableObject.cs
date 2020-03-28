using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SwitchableSelectableObject : SelectableObject
{
    Animator anim;
    bool opened;
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
        StartAnimation();
    }

    void StartAnimation()
    {

        if (!isAnimationOn)
        {
            if (!opened)
            {
                opened = true;
                anim.SetFloat(directionParamName, 1f);
                anim.Play(switchStateName, -1, 0f);
            }
            else
            {
                opened = false;
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