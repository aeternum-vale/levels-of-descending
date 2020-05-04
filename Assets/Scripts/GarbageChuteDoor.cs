using UnityEngine;

public class GarbageChuteDoor : SwitchableObject
{
    bool isUnhinged;
    static readonly string removalStateName = "Removal";
    GameObject rigidDoor;

    protected override void Start()
    {
        base.Start();

        rigidDoor = transform.parent.Find("rigid_garbage_chute_door").gameObject;
    }
    public void Unhinge()
    {
        isUnhinged = true;
        //animationStateName = removalStateName;
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        if (isUnhinged)
        {
            IsGlowingEnabled = false;
        }
    }

    protected override void OnAnimationEnd()
    {
        base.OnAnimationEnd();

        if (isUnhinged)
        {
            gameObject.SetActive(false);
            rigidDoor.SetActive(true);
        }
    }
}
