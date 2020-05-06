using UnityEngine;

public class Scalpel : InventoryObject
{
    Animator anim;
    protected override void Awake()
    {
        base.Awake();
        IsGlowingEnabled = false;
        anim = GetComponent<Animator>();
    }

    public void Emerge()
    {
        gameObject.SetActive(true);
        anim.SetTrigger("Active");
    }

    void OnEmergeAnimationEnd()
    {
        IsGlowingEnabled = true;
    }
}
