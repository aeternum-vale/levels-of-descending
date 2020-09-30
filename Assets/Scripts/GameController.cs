﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdGeneratorModule;
using FloorModule;
using InventoryModule;
using PlayerModule;
using Plugins;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private const float FloorHeight = 3.99f;
    private const int FloorCount = 5;

    private readonly Dictionary<Floor, GameObject> _ground1Colliders = new Dictionary<Floor, GameObject>();

    private int _fakeFloorNumber = 7;

    private Floor[] _floors;
    private int _floorsHalf;
    private int _highestFloorIndex;
    private Player _player;
    private int _realFloorNumber = 7;

    [SerializeField] private AdGenerator adGenerator;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject playerGameObject;
    private int LowestFloorIndex => (_highestFloorIndex + 1) % FloorCount;

    private void Start()
    {
        _highestFloorIndex = FloorCount - 1;
        _floors = new Floor[FloorCount];

        for (int i = 0; i < FloorCount; i++)
        {
            _floors[i] = GenerateRandomFloor(floorPrefab.transform.position + new Vector3(0, FloorHeight * i, 0));
            _ground1Colliders.Add(_floors[i],
                _floors[i].transform.Find(GameConstants.collidersObjectName)
                    .Find(GameConstants.ground1ColliderObjectName).gameObject);
        }

        _floorsHalf = FloorCount / 2;

        Floor initPlayerFloor = _floors[_floorsHalf];
        initPlayerFloor.SetFloorDrawnNumber(_fakeFloorNumber--);

        Vector3 localPosition = playerGameObject.transform.localPosition;

        localPosition = new Vector3(
            localPosition.x,
            initPlayerFloor.transform.localPosition.y,
            localPosition.z
        );

        playerGameObject.transform.localPosition = localPosition;

        initPlayerFloor.SetFrontWallRandomAd();

        foreach (Floor floor in _floors)
            floor.GenerateRandomTextureProjectorsAndGarbageProps();
    }

    private void Awake()
    {
        _player = playerGameObject.GetComponent<Player>();

        Messenger.AddListener(Events.FloorWasTouched, OnFloorWasTouched);
        Messenger.AddListener(Events.InventoryWasUpdated, OnInventoryWasUpdated);
        Messenger.AddListener(Events.CowCodeActivated, OnCowCodeActivated);
        Messenger.AddListener(Events.ElevatorFloorWasTouched, OnElevatorFloorWasTouched);
        Messenger.AddListener(Events.PlayerCutSceneMoveCompleted, OnPlayerCutSceneMoveCompleted);
        Messenger.AddListener(Events.Elevating, OnElevating);
    }

    private bool IsFloorCurrent(Floor floor)
    {
        return _player.LastGround1ColliderTouched == _ground1Colliders[floor];
    }

    private Floor GetNextHigherFloor()
    {
        int i = GetCurrentFloorIndex();
        return _floors[(i + 1) % FloorCount];
    }

    private Floor GetNextLowerFloor()
    {
        int i = GetCurrentFloorIndex();
        return _floors[i - 1 >= 0 ? i - 1 : FloorCount - 1];
    }

    private Floor GetCurrentFloor()
    {
        return _floors[GetCurrentFloorIndex()];
    }

    private int GetCurrentFloorIndex()
    {
        for (int i = 0; i < _floors.Length; i++)
            if (IsFloorCurrent(_floors[i]))
                return i;

        throw new Exception("Can't compute current floor index");
    }

    private int GetFloorIndex(Floor floor)
    {
        for (int i = 0; i < _floors.Length; i++)
            if (_floors[i] == floor)
                return i;
        throw new Exception("Can't compute provided floor index");
    }

    private byte GetFloorDistanceToPlayer(Floor floor)
    {
        int dist = Math.Abs(GetFloorIndex(floor) - GetCurrentFloorIndex());
        return (byte) Math.Min(dist, FloorCount - dist);
    }

    private bool? IsPlayerGoingUp()
    {
        if (_player.PrevLastGround1ColliderTouched == null) return null;

        return _player.PrevLastGround1ColliderTouched.transform.position.y <
               _player.LastGround1ColliderTouched.transform.position.y;
    }

    private Floor GenerateRandomFloor(Vector3 position)
    {
        Floor floor = Instantiate(floorPrefab).GetComponent<Floor>();
        floor.AdGenerator = adGenerator;
        floor.transform.position = position;
        floor.UpdateDoors();
        return floor;
    }

    private void RandomizeFloor(Floor floor)
    {
        //updateFloorDoors (floor);
    }

    private void OnFloorWasTouched()
    {
        _fakeFloorNumber++;

        UpdateRealFloorNumber();
        RearrangeFloors();
        FloorsSelectiveUpdate();
    }

    private void UpdateRealFloorNumber()
    {
        var isPlayerGoingUpCopy = IsPlayerGoingUp();
        if (isPlayerGoingUpCopy.HasValue)
            _realFloorNumber += isPlayerGoingUpCopy.GetValueOrDefault() ? 1 : -1;
    }

    private void RearrangeFloors()
    {
        var isPlayerGoingUpCopy = IsPlayerGoingUp();
        if (!isPlayerGoingUpCopy.HasValue) return;

        if (isPlayerGoingUpCopy.GetValueOrDefault())
        {
            _floors[LowestFloorIndex].transform.localPosition =
                _floors[_highestFloorIndex].transform.localPosition + new Vector3(0, FloorHeight, 0);
            RandomizeFloor(_floors[LowestFloorIndex]);

            _highestFloorIndex = LowestFloorIndex;
        }
        else
        {
            _floors[_highestFloorIndex].transform.localPosition =
                _floors[LowestFloorIndex].transform.localPosition + new Vector3(0, -FloorHeight, 0);
            RandomizeFloor(_floors[LowestFloorIndex]);

            _highestFloorIndex = _highestFloorIndex == 0 ? FloorCount - 1 : _highestFloorIndex - 1;
        }
    }

    private void OnInventoryWasUpdated()
    {
        UpdateInventoryObjectsPresence();
    }

    private void OnCowCodeActivated()
    {
        if (!inventory.Contains(EInventoryItemId.SCALPEL)) GetCurrentFloor().EmergeScalpel();
    }

    private void FloorsSelectiveUpdate()
    {
        ForEachFloorExceptCurrent(floor =>
        {
            byte floorDistanceToPlayer = GetFloorDistanceToPlayer(floor);

            if (floorDistanceToPlayer <= 0 || floorDistanceToPlayer > 2) return;

            floor.ReturnAllObjectsToInitState(floorDistanceToPlayer);

            if (floorDistanceToPlayer == 2)
            {
                floor.GenerateRandomTextureProjectorsAndGarbageProps();
            }

            if (floorDistanceToPlayer == 1)
            {
                floor.SetFrontWallRandomAd();
            }
        });

        UpdateInventoryObjectsPresence();
        ImplementFloorMarksAndFakeFloorNumber();
    }

    private void UpdateInventoryObjectsPresence()
    {
        ForEachFloorExceptCurrent(floor =>
        {
            floor.HideAllInventoryObjects();

            if (!inventory.Contains(EInventoryItemId.ELEVATOR_CALLER_PANEL))
                floor.ShowInventoryObject(EInventoryItemId.ELEVATOR_CALLER_PANEL);

            if (!inventory.Contains(EInventoryItemId.ELEVATOR_CALLER_BUTTON))
                floor.ShowInventoryObject(EInventoryItemId.ELEVATOR_CALLER_BUTTON);

            if (!inventory.Contains(EInventoryItemId.INSULATING_TAPE))
                floor.ShowInventoryObject(EInventoryItemId.INSULATING_TAPE);
        });
    }

    private void ForEachFloorExceptCurrent(Action<Floor> action)
    {
        foreach (Floor floor in _floors)
        {
            if (IsFloorCurrent(floor))
                continue;

            action.Invoke(floor);
        }
    }

    private void ImplementFloorMarksAndFakeFloorNumber()
    {
        int nextFakeFloorNumber = _fakeFloorNumber + 1;

        Floor nextHighFloor = GetNextHigherFloor();
        Floor nextLowFloor = GetNextLowerFloor();

        nextHighFloor.SetFloorDrawnNumber(nextFakeFloorNumber);
        nextLowFloor.SetFloorDrawnNumber(nextFakeFloorNumber);

        nextHighFloor.ResetAllMarks();
        nextLowFloor.ResetAllMarks();

        foreach (var floorMarkKeyValuePair in GameConstants.floorMarks)
        {
            EFloorMarkId id = floorMarkKeyValuePair.Key;
            FloorMark floorMarkValue = floorMarkKeyValuePair.Value;

            if (!floorMarkValue.IsFloorMarked(nextFakeFloorNumber)) continue;

            bool playerHaveAllAssociatedInventoryItems =
                floorMarkValue.AssociatedInventoryItems.All(itemId => inventory.Contains(itemId));

            if (playerHaveAllAssociatedInventoryItems) continue;

            nextHighFloor.SetMark(id);
            nextLowFloor.SetMark(id);

            foreach (EInventoryItemId inventoryItemId in floorMarkValue.AssociatedInventoryItems)
            {
                if (inventory.Contains(inventoryItemId)) continue;

                nextHighFloor.ShowInventoryObject(inventoryItemId);
                nextLowFloor.ShowInventoryObject(inventoryItemId);
            }

            return;
        }
    }

    private void OnElevatorFloorWasTouched()
    {
        StartEndingCutScene();
    }

    private void StartEndingCutScene()
    {
        Transform playerPhTransform = GetCurrentFloor().PlayerPlaceholder.transform;
        Transform cameraPhTransform = GetCurrentFloor().PlayerPlaceholder.transform.GetChild(0);

        _player.CutSceneMoveToPosition(playerPhTransform.position, playerPhTransform.eulerAngles,
            cameraPhTransform.eulerAngles);
    }

    private void OnPlayerCutSceneMoveCompleted()
    {
        ForEachFloorExceptCurrent(floor => floor.HideElevator());
        
        Floor f = GetCurrentFloor();
        f.CloseAndElevateElevator();
        _player.BindYTo(f.Elevator.gameObject);
    }

    private void OnElevating()
    {
        _player.Blackout();
    }

}