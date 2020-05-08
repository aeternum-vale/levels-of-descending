using System;
using System.Collections.Generic;
using AdGeneratorModule;
using DoorModule;
using FloorModule;
using InventoryModule;
using PlayerModule;
using Plugins;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private const float FloorHeight = 3.99f;
    private const int FloorCount = 6;
    private const string EntrywayObjectName = "entryway";
    private const string LeftDoorBaseObjectName = "door_left";
    private const string RightDoorBaseObjectName = "door_right";
    private const string LeftDoorObjectName = "left_door_prefab";
    private const string RightDoorObjectName = "right_door_prefab";

    private readonly Dictionary<Floor, GameObject> _floorToGround1ColliderDict = new Dictionary<Floor, GameObject>();

    private int _fakeFloorNumber = 7;
    private Floor[] _floors;
    private int _floorsHalf;

    private int _highestFloorIndex;

    private Player _player;
    private int _realFloorNumber = 7;

    [SerializeField] private AdGenerator adGenerator;
    [SerializeField] private DoorFactory doorFactory;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject playerGameObject;
    private int LowestFloorIndex => (_highestFloorIndex + 1) % FloorCount;

    private void Start()
    {
        _highestFloorIndex = FloorCount - 1;
        _floors = new Floor[FloorCount];

        for (var i = 0; i < FloorCount; i++)
        {
            _floors[i] = GenerateRandomFloor(floorPrefab.transform.position + new Vector3(0, FloorHeight * i, 0));
            _floorToGround1ColliderDict.Add(_floors[i],
                _floors[i].transform.Find(GameConstants.collidersObjectName)
                    .Find(GameConstants.ground1ColliderObjectName).gameObject);
        }

        _floorsHalf = (int) Mathf.Floor(FloorCount / 2);
        _floors[_floorsHalf].SetFloorDrawnNumber(_fakeFloorNumber--);

        var localPosition = playerGameObject.transform.localPosition;

        localPosition = new Vector3(
            localPosition.x,
            _floors[_floorsHalf].transform.localPosition.y,
            localPosition.z
        );

        playerGameObject.transform.localPosition = localPosition;

        //adGenerator = transform.parent.Find("AdGenerator").gameObject.GetComponent<AdGenerator>();
    }

    private void Awake()
    {
        _player = playerGameObject.GetComponent<Player>();

        Messenger.AddListener(Events.FloorWasTouched, OnFloorWasTouched);
        Messenger.AddListener(Events.InventoryWasUpdated, OnInventoryWasUpdated);
        Messenger<Door>.AddListener(Events.DragonflyCodeActivated, OnDragonflyCodeActivated);
    }

    private Floor GetNextHigherFloor()
    {
        return GetSpecificFloor((i, floorCount) => (i + 1) % FloorCount);
    }

    private Floor GetNextLowerFloor()
    {
        return GetSpecificFloor((i, floorCount) => i - 1 >= 0 ? i - 1 : FloorCount - 1);
    }

    private Floor GetCurrentFloor()
    {
        return GetSpecificFloor((i, floorCount) => i);
    }

    private Floor GetSpecificFloor(OnReachedCurrentIndexCallback cb)
    {
        for (var i = 0; i < _floors.Length; i++)
            if (_player.LastGround1ColliderTouched == _floorToGround1ColliderDict[_floors[i]])
                return _floors[cb(i, FloorCount)];
        return null;
    }

    private void UpdateFloorDoors(Floor floor)
    {
        // Transform oldLeftDoorTransform = floor.transform.Find (leftDoorObjectName);
        // Transform oldRightDoorTransform = floor.transform.Find (rightDoorObjectName);

        // if (oldLeftDoorTransform) {
        // 	Destroy (oldLeftDoorTransform.gameObject);
        // }

        // if (oldRightDoorTransform) {
        // 	Destroy (oldRightDoorTransform.gameObject);
        // }

        var entrywayTransform = floor.transform.Find(EntrywayObjectName);
        var floorLeftDoorBaseTransform = entrywayTransform.Find(LeftDoorBaseObjectName);
        var floorRightDoorBaseTransform = entrywayTransform.Find(RightDoorBaseObjectName);

        var leftDoor = doorFactory.GenerateRandomDoor();
        var rightDoor = doorFactory.GenerateRandomDoor();

        leftDoor.transform.position = floorLeftDoorBaseTransform.position;

        rightDoor.transform.position = floorRightDoorBaseTransform.position;
        rightDoor.Invert();

        leftDoor.name = LeftDoorObjectName;

        rightDoor.name = RightDoorObjectName;

        leftDoor.transform.SetParent(floor.transform);
        rightDoor.transform.SetParent(floor.transform);
    }

    private Floor GenerateRandomFloor(Vector3 position)
    {
        var floor = Instantiate(floorPrefab).GetComponent<Floor>();
        floor.transform.position = position;
        UpdateFloorDoors(floor);
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
        UpdateEnvironment();
    }

    private void UpdateRealFloorNumber()
    {
        if (_player.PrevLastGround1ColliderTouched)
            _realFloorNumber += _player.PrevLastGround1ColliderTouched.transform.position.y <
                                _player.LastGround1ColliderTouched.transform.position.y
                ? 1
                : -1;
    }

    private void RearrangeFloors()
    {
        var localPosition = playerGameObject.transform.localPosition;

        var distToHighestFloor = Mathf.Abs(localPosition.y -
                                           _floors[_highestFloorIndex].transform.localPosition.y);
        var distToLowestFloor =
            Mathf.Abs(localPosition.y -
                      _floors[LowestFloorIndex].transform.localPosition.y) +
            FloorHeight;
        var threshold = _floorsHalf * FloorHeight;

        if (distToHighestFloor < threshold)
        {
            _floors[LowestFloorIndex].transform.localPosition =
                _floors[_highestFloorIndex].transform.localPosition + new Vector3(0, FloorHeight, 0);
            RandomizeFloor(_floors[LowestFloorIndex]);

            _highestFloorIndex = LowestFloorIndex;
        }
        else if (distToLowestFloor < threshold)
        {
            _floors[_highestFloorIndex].transform.localPosition =
                _floors[LowestFloorIndex].transform.localPosition + new Vector3(0, -FloorHeight, 0);
            RandomizeFloor(_floors[LowestFloorIndex]);

            _highestFloorIndex = _highestFloorIndex == 0 ? FloorCount - 1 : _highestFloorIndex - 1;
        }
    }

    private void OnInventoryWasUpdated()
    {
    }

    private void OnDragonflyCodeActivated(Door door)
    {
        if (!inventory.Contains(EInventoryItemId.SCALPEL)) GetCurrentFloor().EmergeScalpel();
    }

    private void UpdateEnvironment()
    {
        foreach (var f in _floors)
        {
            if (_player.LastGround1ColliderTouched &&
                _player.LastGround1ColliderTouched == _floorToGround1ColliderDict[f]
            ) //stop updating of the current floor
                continue;

            foreach (var id in (EInventoryItemId[]) Enum.GetValues(typeof(EInventoryItemId)))
                f.HideObject(GameConstants.inventoryItemToInstancePathMap[id]);

            // foreach (var s in GameConstants..Values.Where(s => s.IsOpened))
            //     s.Switch();

            f.SetFrontWallAd(adGenerator.GetRandomAdTexture());
        }

        var nextFakeFloorNumber = _fakeFloorNumber + 1;

        var nextHighFloor = GetNextHigherFloor();
        var nextLowFloor = GetNextLowerFloor();

        nextHighFloor.SetFloorDrawnNumber(nextFakeFloorNumber);
        nextLowFloor.SetFloorDrawnNumber(nextFakeFloorNumber);

        nextHighFloor.ResetAllMarks();
        nextLowFloor.ResetAllMarks();

        foreach (var floorMarkKeyValuePair in GameConstants.floorMarksDict)
        {
            var id = floorMarkKeyValuePair.Key;
            var floorMarkValue = floorMarkKeyValuePair.Value;

            if (!floorMarkValue.IsFloorMarked(nextFakeFloorNumber)) continue;

            nextHighFloor.SetMark(id);
            nextLowFloor.SetMark(id);

            foreach (var inventoryItem in floorMarkValue.AssociatedInventoryItems)
                if (!inventory.Contains(inventoryItem))
                {
                    nextHighFloor.ShowObject(GameConstants.inventoryItemToInstancePathMap[inventoryItem]);
                    nextLowFloor.ShowObject(GameConstants.inventoryItemToInstancePathMap[inventoryItem]);
                }
        }
    }

    private delegate int OnReachedCurrentIndexCallback(int i, int floorCount);
}