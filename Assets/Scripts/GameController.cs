using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	[SerializeField] private Inventory inventory;

	[SerializeField] private GameObject floorPrefab;
	[SerializeField] private GameObject player;
	[SerializeField] private DoorFactory doorFactory;

	private float floorHeight = 3.99f;
	private int floorCount = 5;

	private Floor[] floors;

	private int highestFloorIndex;
	private int floorsHalf;

	public const string entrywayObjectName = "entryway";
	public const string leftDoorBaseObjectName = "door_left";
	public const string rightDoorBaseObjectName = "door_right";
	public const string leftDoorObjectName = "left_door_prefab";
	public const string rightDoorObjectName = "right_door_prefab";

	void Start () {
		highestFloorIndex = floorCount - 1;
		floors = new Floor[floorCount];

		for (int i = 0; i < floorCount; i++) {
			floors[i] = generateRandomFloor (floorPrefab.transform.position + new Vector3 (0, floorHeight * i, 0));
		}

		floorsHalf = (int) Mathf.Floor (floorCount / 2);

		player.transform.localPosition = new Vector3 (
			player.transform.localPosition.x,
			floors[floorsHalf].transform.localPosition.y,
			player.transform.localPosition.z
		);
	}

	void Awake() {
		Messenger.AddListener(Events.INVENTORY_UPDATED, onInventoryUpdated);
	}

	private void updateFloorDoors (Floor floor) {
		// Transform oldLeftDoorTransform = floor.transform.Find (leftDoorObjectName);
		// Transform oldRightDoorTransform = floor.transform.Find (rightDoorObjectName);

		// if (oldLeftDoorTransform) {
		// 	Destroy (oldLeftDoorTransform.gameObject);
		// }

		// if (oldRightDoorTransform) {
		// 	Destroy (oldRightDoorTransform.gameObject);
		// }

		Transform entrywayTransform = floor.transform.Find (entrywayObjectName);
		Transform floorLeftDoorBaseTransform = entrywayTransform.Find (leftDoorBaseObjectName);
		Transform floorRightDoorBaseTransform = entrywayTransform.Find (rightDoorBaseObjectName);

		Door leftDoor = doorFactory.generateRandomDoor ();
		Door rightDoor = doorFactory.generateRandomDoor ();

		leftDoor.transform.position = floorLeftDoorBaseTransform.position;

		rightDoor.transform.position = floorRightDoorBaseTransform.position;
		rightDoor.invert();

		leftDoor.name = leftDoorObjectName;

		rightDoor.name = rightDoorObjectName;

		leftDoor.transform.SetParent (floor.transform);
		rightDoor.transform.SetParent (floor.transform);

	}

	private Floor generateRandomFloor (Vector3 position) {
		Floor floor = Instantiate (floorPrefab).GetComponent<Floor> ();
		floor.transform.position = position;
		updateFloorDoors (floor);
		return floor;
	}

	private void randomizeFloor (Floor floor) {
		//updateFloorDoors (floor);
	}

	void Update () {
		int lowestIndex = (highestFloorIndex + 1) % floorCount;
		float distToHighestFloor = Mathf.Abs (player.transform.localPosition.y - floors[highestFloorIndex].transform.localPosition.y);
		float distToLowestFloor = Mathf.Abs (player.transform.localPosition.y - floors[lowestIndex].transform.localPosition.y) + floorHeight;
		float threshold = floorsHalf * floorHeight;

		// Debug.Log("distToHighestFloor: " + distToHighestFloor + " and should be < " + threshold);
		// Debug.Log("distToLowestFloor: "+ distToLowestFloor + " and should be < " + threshold);

		if (distToHighestFloor < threshold) {

			floors[lowestIndex].transform.localPosition =
				floors[highestFloorIndex].transform.localPosition + new Vector3 (0, floorHeight, 0);
			randomizeFloor (floors[lowestIndex]);

			highestFloorIndex = lowestIndex;

		} else if (distToLowestFloor < threshold) {

			floors[highestFloorIndex].transform.localPosition =
				floors[lowestIndex].transform.localPosition + new Vector3 (0, -floorHeight, 0);
			randomizeFloor (floors[lowestIndex]);

			highestFloorIndex = highestFloorIndex == 0 ? floorCount - 1 : highestFloorIndex - 1;
		}
	}

	private void onInventoryUpdated() {
		UpdateEnvironment();
	}

	protected void UpdateEnvironment() {
		if (inventory.AvailableItemsDict[EInventoryItemID.POSTBOX_KEY]) {
			for (int i = 0; i < floors.Length; i++) {
				floors[i].removeObject(GameConstants.InventoryInstanceNameMap[EInventoryItemID.POSTBOX_KEY]);
			}
		}
	}
}