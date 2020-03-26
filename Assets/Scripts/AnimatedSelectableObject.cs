using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedSelectableObject : SelectableObject
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnClick(EInventoryObjectID? selectedInventoryObjectId = null)
    {
        base.OnClick(selectedInventoryObjectId);
        StartAnimation();
    }

    void StartAnimation()
    {
        anim.SetTrigger("Active");
    }
}