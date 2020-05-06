using UnityEngine;

public class GarbageChute : MonoBehaviour
{
    GarbageChuteDoor garbageChuteDoor;
    SwitchableObject garbageChuteHinge;
    InventoryObject elevatorButtonPanel;

    private void Start()
    {
        garbageChuteDoor = transform.GetComponentInChildren<GarbageChuteDoor>();
        garbageChuteHinge = transform.Find(SwitchableObject.GetName(ESwitchableObjectID.GARBAGE_CHUTE_DOOR_HINGE)).GetComponent<SwitchableObject>();
        elevatorButtonPanel = transform.Find(InventoryObject.GetName(EInventoryItemID.ELEVATOR_BUTTON_PANEL)).GetComponent<InventoryObject>();

        garbageChuteHinge.OnStateReached += OnGarbageChuteStateReached;
    }

    private void OnGarbageChuteStateReached(object sender, MultiStateObjectEventArgs e)
    {
        if (e.stateId == (byte)ESwitchableObjectStateId.OPEN)
        {
            garbageChuteDoor.Unhinge();
            elevatorButtonPanel.IsGrabable = true;
        }
    }
}
