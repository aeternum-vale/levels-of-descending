﻿using InventoryModule;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class GarbageChute : MonoBehaviour
    {
        private GarbageChuteDoor _garbageChuteDoor;
        private SwitchableObject _garbageChuteHinge;
        private InventoryObject _elevatorButtonPanel;

        private void Start()
        {
            _garbageChuteDoor = transform.GetComponentInChildren<GarbageChuteDoor>();
            _garbageChuteHinge = transform.Find(SwitchableObject.GetName(ESwitchableObjectId.GARBAGE_CHUTE_DOOR_HINGE))
                .GetComponent<SwitchableObject>();
            _elevatorButtonPanel = transform.Find(InventoryObject.GetName(EInventoryItemId.ELEVATOR_BUTTON_PANEL))
                .GetComponent<InventoryObject>();

            _garbageChuteHinge.OnStateReached += OnGarbageChuteStateReached;
        }

        private void OnGarbageChuteStateReached(object sender, MultiStateObjectEventArgs e)
        {
            if (e.StateId == (byte) ESwitchableObjectStateId.OPEN)
            {
                _garbageChuteDoor.Unhinge();
                _elevatorButtonPanel.IsGrabable = true;
            }
        }
    }
}