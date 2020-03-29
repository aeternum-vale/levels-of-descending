using System;
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

    readonly float floorHeight = 3.99f;
    readonly int floorCount = 5;

    Floor[] floors;

    int highestFloorIndex;
    int floorsHalf;

    public const string entrywayObjectName = "entryway";
    public const string leftDoorBaseObjectName = "door_left";
    public const string rightDoorBaseObjectName = "door_right";
    public const string leftDoorObjectName = "left_door_prefab";
    public const string rightDoorObjectName = "right_door_prefab";

    void Start()
    {
        highestFloorIndex = floorCount - 1;
        floors = new Floor[floorCount];

        for (int i = 0; i < floorCount; i++)
        {
            floors[i] = GenerateRandomFloor(floorPrefab.transform.position + new Vector3(0, floorHeight * i, 0));
        }

        floorsHalf = (int)Mathf.Floor(floorCount / 2);

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

        Door leftDoor = doorFactory.generateRandomDoor();
        Door rightDoor = doorFactory.generateRandomDoor();

        leftDoor.transform.position = floorLeftDoorBaseTransform.position;

        rightDoor.transform.position = floorRightDoorBaseTransform.position;
        rightDoor.invert();

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
        RearrangeFloors();
        UpdateEnvironment();
    }

    void RearrangeFloors()
    {
        int lowestIndex = (highestFloorIndex + 1) % floorCount;
        float distToHighestFloor = Mathf.Abs(playerGameObject.transform.localPosition.y - floors[highestFloorIndex].transform.localPosition.y);
        float distToLowestFloor = Mathf.Abs(playerGameObject.transform.localPosition.y - floors[lowestIndex].transform.localPosition.y) + floorHeight;
        float threshold = floorsHalf * floorHeight;

        // Debug.Log("distToHighestFloor: " + distToHighestFloor + " and should be < " + threshold);
        // Debug.Log("distToLowestFloor: "+ distToLowestFloor + " and should be < " + threshold);

        if (distToHighestFloor < threshold)
        {

            floors[lowestIndex].transform.localPosition =
                floors[highestFloorIndex].transform.localPosition + new Vector3(0, floorHeight, 0);
            RandomizeFloor(floors[lowestIndex]);

            highestFloorIndex = lowestIndex;

        }
        else if (distToLowestFloor < threshold)
        {

            floors[highestFloorIndex].transform.localPosition =
                floors[lowestIndex].transform.localPosition + new Vector3(0, -floorHeight, 0);
            RandomizeFloor(floors[lowestIndex]);

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
            if (player.LastFloorTouched && player.LastFloorTouched == floors[i].transform.Find("colliders").Find("floor").gameObject)
            {
                continue;
            }


            if (inventory.AvailableItemsDict.ContainsKey(EInventoryItemID.POSTBOX_KEY))
            {
                floors[i].removeObject(GameConstants.InventoryInstanceNameMap[EInventoryItemID.POSTBOX_KEY]);
            }

            SwitchableSelectableObject s = floors[i].transform.Find("pad").gameObject.GetComponent<SwitchableSelectableObject>();
            if (s.IsOpened)
            {
                s.Switch();
            }
        }
    }
}