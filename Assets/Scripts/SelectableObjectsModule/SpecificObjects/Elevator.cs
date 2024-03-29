﻿using System;
using Plugins;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class Elevator : MonoBehaviour, IInitStateReturnable
    {
        private static readonly int ElevatorOpenStateNameHash = Animator.StringToHash("Open");
        private static readonly int ElevatorCloseAndElevateStateNameHash = Animator.StringToHash("Elevate");

        private ElevatorCaller _caller;
        private Animator _animator;
        private GameObject _elevatorRoom;

        private bool _isDoorsOpened;

        public int InitStateSafeDistanceToPlayer { get; set; }

        public void ReturnToInitState(int floorDistanceToPlayer)
        {
            if (floorDistanceToPlayer <= 1) return;
            
            _animator.Play(GameConstants.idleStateNameHash, -1, 1f);
            _isDoorsOpened = false;
            _elevatorRoom.SetActive(false);
        }

        private void Start()
        {
            _caller = transform.GetComponentInChildren<ElevatorCaller>();
            _animator = transform.GetComponent<Animator>();
            _elevatorRoom = transform.Find("elevator_room").gameObject;

            _caller.CallIsDone += CallIsDone;
        }

        private void CallIsDone(object sender, EventArgs e)
        {
            if (_isDoorsOpened) return;

            _isDoorsOpened = true;

            _elevatorRoom.SetActive(true);
            _animator.Play(ElevatorOpenStateNameHash, -1, 0f);
        }

        public void CloseAndElevate()
        {
            _animator.Play(ElevatorCloseAndElevateStateNameHash, -1, 0f);
        }

        private void OnElevating()
        {
            Messenger.Broadcast(Events.Elevating);
        }
    }
}