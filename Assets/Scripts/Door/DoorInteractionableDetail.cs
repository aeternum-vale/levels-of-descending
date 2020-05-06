using UnityEngine;

public class DoorInteractionableDetail : AnimatedSelectableObject
{
    Door parentDoor;
    [SerializeField] EDoorAction action;

    protected override void Start()
    {
        base.Start();
        parentDoor = transform.parent.parent.gameObject.GetComponent<Door>();
    }
    public override void OnClick(EInventoryItemID? selectedInventoryItemId, GameObject colliderCarrier)
    {
        base.OnClick(selectedInventoryItemId, colliderCarrier);
        parentDoor.Interact(action);
    }
}
