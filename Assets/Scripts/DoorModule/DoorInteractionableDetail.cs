using SelectableObjectsModule;
using UnityEngine;

namespace DoorModule
{
    public class DoorInteractionableDetail : PushableObject
    {
        private Door _parentDoor;
        [SerializeField] private EDoorAction action;

        protected override void Start()
        {
            base.Start();
            _parentDoor = transform.parent.parent.gameObject.GetComponent<Door>();
        }

        protected override void OnPush()
        {
            base.OnPush();
            _parentDoor.Interact(action);
        }
    }
}