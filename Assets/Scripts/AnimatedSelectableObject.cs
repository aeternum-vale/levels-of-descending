using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedSelectableObject : SelectableObject
{
    Animator anim;

    protected virtual void Start()
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
        anim.SetTrigger("Active");
    }
}