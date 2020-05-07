using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class GarbageChuteDoor : SwitchableObject
    {
        private const string RemovingStateName = "Removing";
        private const byte RemovingStateId = 2;
        private bool _isUnhinged;
        private GameObject _rigidDoor;

        protected override void Start()
        {
            base.Start();
            _rigidDoor = transform.parent.Find("rigid_garbage_chute_door").gameObject;
        }

        protected override void Awake()
        {
            base.Awake();

            States.Add(new GraphState {Name = RemovingStateName, OnReached = OnRemove});
            StateTransitions[(byte) ESwitchableObjectStateId.CLOSE][0].Condition = () => !_isUnhinged;

            StateTransitions[(byte) ESwitchableObjectStateId.CLOSE].Add(new GraphTransition
                {NextStateId = RemovingStateId, Condition = () => _isUnhinged});
        }

        public void Unhinge()
        {
            _isUnhinged = true;
        }

        private void OnRemove()
        {
            if (_isUnhinged) IsGlowingEnabled = false;
        }

        protected override void OnAnimationEnd()
        {
            base.OnAnimationEnd();

            if (_isUnhinged) _rigidDoor.SetActive(true);
        }
    }
}