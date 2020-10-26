using System;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class GarbageChute : MonoBehaviour, IInitStateReturnable
    {
        private static readonly int RemoveStateNameHash = Animator.StringToHash("Remove");
        private SwitchableObject _door;
        private Rigidbody _doorRigidbody;

        private Vector3 _doorRigidbodyInitPosition;
        private Quaternion _doorRigidbodyInitRotation;

        private InventoryObject _elevatorButtonPanel;
        private SwitchableObject _hinge;
        public int InitStateSafeDistanceToPlayer { get; set; } = 2;

        public void ReturnToInitState(int floorDistanceToPlayer)
        {
            if (floorDistanceToPlayer < InitStateSafeDistanceToPlayer) return;

            _door.gameObject.SetActive(true);
            _door.IsMuted = false;
            _door.AnimationNameHash = SwitchableObject.defaultSwitchStateNameHash;
            _door.OpenAnimationCompleted -= OnDoorOpenAnimationCompleted;
            _door.IsGlowingEnabled = true;
            _door.Close();

            _elevatorButtonPanel.IsGrabable = false;
            _doorRigidbody.gameObject.SetActive(false);
            _doorRigidbody.velocity = new Vector3(0f, 0f, 0f);
            _doorRigidbody.angularVelocity = new Vector3(0f, 0f, 0f);

            Transform doorRigidbodyTransform = _doorRigidbody.transform;
            doorRigidbodyTransform.localPosition = _doorRigidbodyInitPosition;
            doorRigidbodyTransform.localRotation = _doorRigidbodyInitRotation;

            _hinge.gameObject.SetActive(true);
        }

        private void Start()
        {
            _door = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.GARBAGE_CHUTE_DOOR);
            _hinge = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.GARBAGE_CHUTE_DOOR_HINGE);
            _elevatorButtonPanel = SelectableObject.GetAsChild(gameObject, EInventoryItemId.ELEVATOR_CALLER_PANEL);
            _doorRigidbody = transform.Find("rigid_garbage_chute_door").GetComponent<Rigidbody>();

            _door.InitStateSafeDistanceToPlayer = InitStateSafeDistanceToPlayer;
            _hinge.InitStateSafeDistanceToPlayer = InitStateSafeDistanceToPlayer;

            GameObject doorRigidbodyGameObject = _doorRigidbody.gameObject;

            _doorRigidbodyInitPosition = doorRigidbodyGameObject.transform.localPosition;
            _doorRigidbodyInitRotation = doorRigidbodyGameObject.transform.localRotation;

            _hinge.OpenCondition = () => !_door.IsOpened;
            _hinge.Opened += OnUnhinged;
        }

        private void OnUnhinged(object s, EventArgs e)
        {
            _door.IsMuted = true;
            _door.AnimationNameHash = RemoveStateNameHash;
            _door.OpenAnimationCompleted += OnDoorOpenAnimationCompleted;
            _elevatorButtonPanel.IsGrabable = true;
            _hinge.gameObject.SetActive(false);
        }

        private void OnDoorOpenAnimationCompleted(object sender, EventArgs e)
        {
            _door.gameObject.SetActive(false);
            _doorRigidbody.gameObject.SetActive(true);
            //_garbageChuteDoor.IsGlowingEnabled = false;
        }
    }
}