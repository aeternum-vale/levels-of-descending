using System;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class GarbageChute : MonoBehaviour
    {
        private static readonly int RemoveStateNameHash = Animator.StringToHash("Remove");
        private InventoryObject _elevatorButtonPanel;
        private SwitchableObject _garbageChuteDoor;
        private SwitchableObject _garbageChuteHinge;
        private GameObject _rigidDoor;

        private void Start()
        {
            _garbageChuteDoor = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.GARBAGE_CHUTE_DOOR);
            _garbageChuteHinge = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.GARBAGE_CHUTE_DOOR_HINGE);
            _elevatorButtonPanel = SelectableObject.GetAsChild(gameObject, EInventoryItemId.ELEVATOR_CALLER_PANEL);
            _rigidDoor = transform.Find("rigid_garbage_chute_door").gameObject;

            _garbageChuteHinge.OpenCondition = () => !_garbageChuteDoor.IsOpened;
            _garbageChuteHinge.Opened += OnUnhinged;
        }

        private void OnUnhinged(object s, EventArgs e)
        {
            _garbageChuteDoor.AnimationNameHash = RemoveStateNameHash;

            _garbageChuteDoor.OpenAnimationCompleted += (sender, args) =>
            {
                _rigidDoor.SetActive(true);
                _garbageChuteDoor.IsGlowingEnabled = false;
            };

            _elevatorButtonPanel.IsGrabable = true;
        }
    }
}