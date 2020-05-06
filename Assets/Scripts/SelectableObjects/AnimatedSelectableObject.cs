using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedSelectableObject : SelectableObject
{
    Animator anim;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnClick(EInventoryItemID? selectedInventoryItemId, GameObject colliderCarrier)
    {
        base.OnClick(selectedInventoryItemId, colliderCarrier);
        StartAnimation();
    }

    void StartAnimation()
    {
        anim.SetTrigger("Active");
    }
}