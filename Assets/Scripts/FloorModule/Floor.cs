﻿using System.Collections.Generic;
using System.Linq;
using DoorModule;
using SelectableObjectsModule;
using SelectableObjectsModule.SpecificObjects;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace FloorModule
{
    public class Floor : MonoBehaviour
    {
        private const string FrontWallName = "front_wall";
        private const string EntrywayObjectName = "entryway";
        private const string LeftDoorBaseObjectName = "door_left";
        private const string RightDoorBaseObjectName = "door_right";
        private const string LeftDoorObjectName = "left_door_prefab";
        private const string RightDoorObjectName = "right_door_prefab";
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int IsTitleOn = Shader.PropertyToID("_IsTitleOn");
        private static readonly int ActiveTextureNumber = Shader.PropertyToID("_ActiveTextureNumber");
        private static readonly int FloorNumber = Shader.PropertyToID("_FloorNumber");

        private readonly Dictionary<EInventoryItemId, InventoryObject> _inventoryObjects =
            new Dictionary<EInventoryItemId, InventoryObject>();

        private readonly Dictionary<EFloorMarkId, bool> _markStatesDict = new Dictionary<EFloorMarkId, bool>
        {
            {EFloorMarkId.DRAGONFLY, false},
            {EFloorMarkId.LOST_PET_SIGN, false}
        };

        private Material _adMaterial;
        private Material _frontWallMaterial;
        private Door _leftDoor;
        private Material _postboxPartMaterialWithDragonFly;

        private IInitStateReturnable[] _returnableObjects;
        private Door _rightDoor;

        private Scalpel _scalpel;

        [SerializeField] private DoorFactory doorFactory;

        private void Start()
        {
            _postboxPartMaterialWithDragonFly =
                transform.Find("postbox/Cube.003").GetComponent<MeshRenderer>().material;
            _leftDoor = transform.Find("left_door_prefab").GetComponent<Door>();
            _rightDoor = transform.Find("right_door_prefab").GetComponent<Door>();
            _scalpel = SelectableObject.GetAsChild<Scalpel>(gameObject, EInventoryItemId.SCALPEL);

            _returnableObjects = transform.GetComponentsInChildren<IInitStateReturnable>(true);

            transform.GetComponentsInChildren<InventoryObject>(true)
                .ToList()
                .ForEach(io => _inventoryObjects.Add(io.objectId, io));
        }

        private void Awake()
        {
            _frontWallMaterial = transform.Find(GameConstants.entrywayObjectName).Find(FrontWallName)
                .GetComponent<MeshRenderer>().material;
            _adMaterial = transform.Find(SelectableObject.GetPath(ESwitchableObjectId.AD)).GetComponent<MeshRenderer>()
                .material;
        }

        public void ReturnAllObjectsToInitState()
        {
            _returnableObjects.ToList().ForEach(returnable => returnable.ReturnToInitState());
        }

        private void SetActivityOfInventoryObject(EInventoryItemId id, bool active)
        {
            _inventoryObjects[id].gameObject.SetActive(active);
        }

        public void HideInventoryObject(EInventoryItemId id)
        {
            SetActivityOfInventoryObject(id, false);
        }

        public void ShowInventoryObject(EInventoryItemId id)
        {
            SetActivityOfInventoryObject(id, true);
        }

        public void HideAllInventoryObjects()
        {
            _inventoryObjects.Keys.ToList().ForEach(HideInventoryObject);
        }

        public void SetFloorDrawnNumber(int number)
        {
            _frontWallMaterial.SetFloat(FloorNumber, number);
        }

        public void EmergeScalpel()
        {
            _scalpel.Emerge();
        }

        public void SetMark(EFloorMarkId id)
        {
            _markStatesDict[id] = true;

            switch (id)
            {
                case EFloorMarkId.DRAGONFLY:
                    _postboxPartMaterialWithDragonFly.SetFloat(IsTitleOn, 1f);
                    _leftDoor.MarkWithDragonfly();
                    break;

                case EFloorMarkId.LOST_PET_SIGN:
                    _adMaterial.SetFloat(ActiveTextureNumber, 1f);
                    break;
            }
        }

        public void ResetMark(EFloorMarkId id)
        {
            _markStatesDict[id] = false;

            switch (id)
            {
                case EFloorMarkId.DRAGONFLY:
                    _postboxPartMaterialWithDragonFly.SetFloat(IsTitleOn, 0f);
                    _leftDoor.UnmarkWithDragonfly();
                    break;
            }
        }

        public void ResetAllMarks()
        {
            foreach (var key in _markStatesDict.Keys.ToList()) ResetMark(key);
        }

        public void SetFrontWallAd(Texture2D texture)
        {
            _adMaterial.SetTexture(MainTex, texture);
        }

        public void UpdateDoors()
        {
            var entrywayTransform = transform.Find(EntrywayObjectName);
            var floorLeftDoorBaseTransform = entrywayTransform.Find(LeftDoorBaseObjectName);
            var floorRightDoorBaseTransform = entrywayTransform.Find(RightDoorBaseObjectName);

            var leftDoor = doorFactory.GenerateRandomDoor();
            var rightDoor = doorFactory.GenerateRandomDoor();

            leftDoor.transform.position = floorLeftDoorBaseTransform.position;

            rightDoor.transform.position = floorRightDoorBaseTransform.position;
            rightDoor.Invert();

            leftDoor.name = LeftDoorObjectName;
            rightDoor.name = RightDoorObjectName;

            var transformValue = transform;
            leftDoor.transform.SetParent(transformValue);
            rightDoor.transform.SetParent(transformValue);
        }
    }
}