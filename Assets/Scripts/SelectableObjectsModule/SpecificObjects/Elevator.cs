using System;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class Elevator : MonoBehaviour, IInitStateReturnable
    {
        private ElevatorCaller _caller;
        private Animator _doorsAnimator;
        private GameObject _elevatorBase;

        private bool _isDoorsOpened;

        public int InitStateSafeDistanceToPlayer { get; set; }

        public void ReturnToInitState(int floorDistanceToPlayer)
        {
            _doorsAnimator.Play("Idle", -1, 1f);
        }

        private void Start()
        {
            _caller = transform.GetComponentInChildren<ElevatorCaller>();
            _doorsAnimator = transform.Find("doors").GetComponent<Animator>();
            _elevatorBase = transform.Find("base").gameObject;

            _caller.CallIsDone += CallIsDone;
        }

        private void CallIsDone(object sender, EventArgs e)
        {
            if (_isDoorsOpened) return;

            _isDoorsOpened = true;

            _elevatorBase.SetActive(true);
            _doorsAnimator.Play("Open", -1, 0f);
        }
    }
}