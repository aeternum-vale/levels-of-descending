using InventoryModule;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class GarbageChute : MonoBehaviour
    {
        private InventoryObject _elevatorButtonPanel;
        private GarbageChuteDoor _garbageChuteDoor;
        private SwitchableObject _garbageChuteHinge;

        private void Start()
        {
            _garbageChuteDoor = transform.GetComponentInChildren<GarbageChuteDoor>();
            _garbageChuteHinge = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.GARBAGE_CHUTE_DOOR_HINGE);

            _elevatorButtonPanel = SelectableObject.GetAsChild(gameObject, EInventoryItemId.ELEVATOR_CALLER_PANEL);

            _garbageChuteHinge.States[(byte) ESwitchableObjectStateId.OPEN].OnReached +=
                OnGarbageChuteHingeOpenStateReached;
        }

        private void OnGarbageChuteHingeOpenStateReached()
        {
            _garbageChuteDoor.Unhinge();
            _elevatorButtonPanel.IsGrabable = true;
        }
    }
}