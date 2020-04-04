﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject playerGameObject;
    [SerializeField] DoorFactory doorFactory;

    Player player;
    Floor[] floors;

    int highestFloorIndex;
    int floorsHalf;
    int fakeFloorCount = 7;
    int LowestFloorIndex => (highestFloorIndex + 1) % floorCount;

    static readonly float floorHeight = 3.99f;
    static readonly int floorCount = 7;
    public static readonly string entrywayObjectName = "entryway";
    public static readonly string leftDoorBaseObjectName = "door_left";
    public static readonly string rightDoorBaseObjectName = "door_right";
    public static readonly string leftDoorObjectName = "left_door_prefab";
    public static readonly string rightDoorObjectName = "right_door_prefab";

    static Dictionary<Floor, GameObject> floorToGround1ColliderDict = new Dictionary<Floor, GameObject>();

    void Start()
    {

        highestFloorIndex = floorCount - 1;
        floors = new Floor[floorCount];

        for (int i = 0; i < floorCount; i++)
        {
            floors[i] = GenerateRandomFloor(floorPrefab.transform.position + new Vector3(0, floorHeight * i, 0));
            floorToGround1ColliderDict.Add(floors[i], floors[i].transform.Find(GameConstants.collidersObjectName).Find(GameConstants.ground1ColliderObjectName).gameObject);
        }

        floorsHalf = (int)Mathf.Floor(floorCount / 2);
        floors[floorsHalf].SetFloorNumber(fakeFloorCount--);

        playerGameObject.transform.localPosition = new Vector3(
            playerGameObject.transform.localPosition.x,
            floors[floorsHalf].transform.localPosition.y,
            playerGameObject.transform.localPosition.z
        );
    }

    void Awake()
    {
        player = playerGameObject.GetComponent<Player>();

        Messenger.AddListener(Events.FLOOR_TOUCHED, OnFloorTouched);
        Messenger.AddListener(Events.INVENTORY_UPDATED, OnInventoryUpdated);
        Messenger<ESwitchableObjectID>.AddListener(Events.SWITCHABLE_OBJECT_OPENED, OnSwitchableObjectOpened);
    }

    void UpdateFloorDoors(Floor floor)
    {
        // Transform oldLeftDoorTransform = floor.transform.Find (leftDoorObjectName);
        // Transform oldRightDoorTransform = floor.transform.Find (rightDoorObjectName);

        // if (oldLeftDoorTransform) {
        // 	Destroy (oldLeftDoorTransform.gameObject);
        // }

        // if (oldRightDoorTransform) {
        // 	Destroy (oldRightDoorTransform.gameObject);
        // }

        Transform entrywayTransform = floor.transform.Find(entrywayObjectName);
        Transform floorLeftDoorBaseTransform = entrywayTransform.Find(leftDoorBaseObjectName);
        Transform floorRightDoorBaseTransform = entrywayTransform.Find(rightDoorBaseObjectName);

        Door leftDoor = doorFactory.GenerateRandomDoor();
        Door rightDoor = doorFactory.GenerateRandomDoor();

        leftDoor.transform.position = floorLeftDoorBaseTransform.position;

        rightDoor.transform.position = floorRightDoorBaseTransform.position;
        rightDoor.Invert();

        leftDoor.name = leftDoorObjectName;

        rightDoor.name = rightDoorObjectName;

        leftDoor.transform.SetParent(floor.transform);
        rightDoor.transform.SetParent(floor.transform);

    }

    Floor GenerateRandomFloor(Vector3 position)
    {
        Floor floor = Instantiate(floorPrefab).GetComponent<Floor>();
        floor.transform.position = position;
        UpdateFloorDoors(floor);
        return floor;
    }

    void RandomizeFloor(Floor floor)
    {
        //updateFloorDoors (floor);
    }

    void OnFloorTouched()
    {
        fakeFloorCount++;
        RearrangeFloors();
        UpdateEnvironment();
    }

    void RearrangeFloors()
    {
        float distToHighestFloor = Mathf.Abs(playerGameObject.transform.localPosition.y - floors[highestFloorIndex].transform.localPosition.y);
        float distToLowestFloor = Mathf.Abs(playerGameObject.transform.localPosition.y - floors[LowestFloorIndex].transform.localPosition.y) + floorHeight;
        float threshold = floorsHalf * floorHeight;

        if (distToHighestFloor < threshold)
        {

            floors[LowestFloorIndex].transform.localPosition =
                floors[highestFloorIndex].transform.localPosition + new Vector3(0, floorHeight, 0);
            RandomizeFloor(floors[LowestFloorIndex]);

            highestFloorIndex = LowestFloorIndex;

        }
        else if (distToLowestFloor < threshold)
        {

            floors[highestFloorIndex].transform.localPosition =
                floors[LowestFloorIndex].transform.localPosition + new Vector3(0, -floorHeight, 0);
            RandomizeFloor(floors[LowestFloorIndex]);

            highestFloorIndex = highestFloorIndex == 0 ? floorCount - 1 : highestFloorIndex - 1;
        }
    }

    void OnInventoryUpdated()
    {
        UpdateEnvironment();
    }

    void OnSwitchableObjectOpened(ESwitchableObjectID id)
    {
    }

    void UpdateEnvironment()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            if (player.LastGround1Touched && player.LastGround1Touched == floorToGround1ColliderDict[floors[i]])
            {
                continue;
            }

            if (inventory.AvailableItemsDict.ContainsKey(EInventoryItemID.POSTBOX_KEY))
            {
                floors[i].RemoveObject(GameConstants.inventoryItemToInstanceNameMap[EInventoryItemID.POSTBOX_KEY]);
            }

            SwitchableSelectableObject s = floors[i].transform.Find(GameConstants.switchableObjectToInstanceNameMap[ESwitchableObjectID.PAD]).gameObject.GetComponent<SwitchableSelectableObject>();
            if (s.IsOpened)
            {
                s.Switch();
            }
        }

        Floor nextFloor = GetNextHigherFloor();
        nextFloor.SetFloorNumber(fakeFloorCount + 1);

        Floor prevFloor = GetNextLowerFloor();
        prevFloor.SetFloorNumber(fakeFloorCount + 1);
    }

    private Floor GetNextHigherFloor()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            if (player.LastGround1Touched == floorToGround1ColliderDict[floors[i]])
            {
                return floors[(i + 1) % floorCount];
            }
        }
        return null;
    }

    private Floor GetNextLowerFloor()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            if (player.LastGround1Touched == floorToGround1ColliderDict[floors[i]])
            {
                return floors[(i - 1 >= 0) ? (i - 1) : (floorCount - 1)];
            }
        }
        return null;
    }

}