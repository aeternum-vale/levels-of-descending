using System;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class GarbageChute : MonoBehaviour, IInitStateReturnable
    {
        private static readonly int RemoveStateNameHash = Animator.StringToHash("Remove");

        private InventoryObject _elevatorButtonPanel;
        private SwitchableObject _garbageChuteDoor;
        private SwitchableObject _garbageChuteHinge;
        private Rigidbody _doorRigidbody;

        private Vector3 _doorRigidbodyInitPosition;
        private Quaternion _doorRigidbodyInitRotation;
        public int InitStateSafeDistanceToPlayer { get; set; } = 2;

        public void ReturnToInitState(int floorDistanceToPlayer)
        {
            if (floorDistanceToPlayer < InitStateSafeDistanceToPlayer) return;

            _garbageChuteDoor.AnimationNameHash = SwitchableObject.defaultSwitchStateNameHash;
            _garbageChuteDoor.OpenAnimationCompleted -= OnGarbageChuteDoorOpenAnimationCompleted;
            _elevatorButtonPanel.IsGrabable = false;

            _doorRigidbody.gameObject.SetActive(false);
            _doorRigidbody.velocity = new Vector3(0f, 0f, 0f);
            _doorRigidbody.angularVelocity = new Vector3(0f, 0f, 0f);

            Transform doorRigidbodyTransform = _doorRigidbody.transform;
            doorRigidbodyTransform.localPosition = _doorRigidbodyInitPosition;
            doorRigidbodyTransform.localRotation = _doorRigidbodyInitRotation;

            _garbageChuteDoor.IsGlowingEnabled = true;
        }

        private void Start()
        {
            _garbageChuteDoor = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.GARBAGE_CHUTE_DOOR);
            _garbageChuteHinge = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.GARBAGE_CHUTE_DOOR_HINGE);
            _elevatorButtonPanel = SelectableObject.GetAsChild(gameObject, EInventoryItemId.ELEVATOR_CALLER_PANEL);
            _doorRigidbody = transform.Find("rigid_garbage_chute_door").GetComponent<Rigidbody>();

            _garbageChuteDoor.InitStateSafeDistanceToPlayer = InitStateSafeDistanceToPlayer;
            _garbageChuteHinge.InitStateSafeDistanceToPlayer = InitStateSafeDistanceToPlayer;

            GameObject doorRigidbodyGameObject = _doorRigidbody.gameObject;

            _doorRigidbodyInitPosition = doorRigidbodyGameObject.transform.localPosition;
            _doorRigidbodyInitRotation = doorRigidbodyGameObject.transform.localRotation;

            _garbageChuteHinge.OpenCondition = () => !_garbageChuteDoor.IsOpened;
            _garbageChuteHinge.Opened += OnUnhinged;
        }

        private void OnUnhinged(object s, EventArgs e)
        {
            _garbageChuteDoor.AnimationNameHash = RemoveStateNameHash;
            _garbageChuteDoor.OpenAnimationCompleted += OnGarbageChuteDoorOpenAnimationCompleted;
            _elevatorButtonPanel.IsGrabable = true;
        }

        private void OnGarbageChuteDoorOpenAnimationCompleted(object sender, EventArgs e)
        {
            _garbageChuteDoor.gameObject.SetActive(false);
            _doorRigidbody.gameObject.SetActive(true);
            //_garbageChuteDoor.IsGlowingEnabled = false;
        }
    }
}