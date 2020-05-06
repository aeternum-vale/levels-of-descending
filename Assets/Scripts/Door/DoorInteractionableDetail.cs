using UnityEngine;

public class DoorInteractionableDetail : PushableObject
{
    Door parentDoor;
    [SerializeField] EDoorAction action;

    protected override void Start()
    {
        base.Start();
        parentDoor = transform.parent.parent.gameObject.GetComponent<Door>();
    }

    protected override void OnPush()
    {
        base.OnPush();
        parentDoor.Interact(action);
    }
}
