using System;
using System.Collections.Generic;
using System.Linq;
using AdGeneratorModule;
using DoorModule;
using FloorModule.PropsGenerator;
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
        private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");
        private static readonly int FloorNumberPropertyId = Shader.PropertyToID("_FloorNumber");
        public static Texture2D LostRabbitAdTexture;

        private readonly Dictionary<EInventoryItemId, InventoryObject> _inventoryObjects =
            new Dictionary<EInventoryItemId, InventoryObject>();

        private readonly Dictionary<EFloorMarkId, bool> _markStates = new Dictionary<EFloorMarkId, bool>
        {
            {EFloorMarkId.DRAGONFLY, false},
            {EFloorMarkId.RABBIT_AD, false},
            {EFloorMarkId.RABBIT_SYMBOL, false}
        };

        private Material _adMaterial;
        private SwitchableObject _ePanelDoor;
        private Material _ePanelDoorMaterial;
        private Material _frontWallMaterial;
        private Door _leftDoor;
        private SwitchableObject _postboxDoor;
        private Material _postboxBaseMaterial;
        private List<IInitStateReturnable> _returnableObjects;
        private Door _rightDoor;
        private Scalpel _scalpel;

        [SerializeField] private DoorController doorController;

        public AdGenerator AdGenerator { get; set; }
        private TextureProjectorPropsGenerator _textureProjectorPropsGenerator;
        private GarbagePropsGenerator _garbagePropsGenerator;
        
        private void Start()
        {
            _postboxBaseMaterial =
                transform.Find("postbox/postbox-base").GetComponent<MeshRenderer>().material;
            _leftDoor = transform.Find("left_door_prefab").GetComponent<Door>();
            _rightDoor = transform.Find("right_door_prefab").GetComponent<Door>();
            _scalpel = SelectableObject.GetAsChild<Scalpel>(gameObject, EInventoryItemId.SCALPEL);
            _ePanelDoor = SelectableObject.GetAsChildByPath(gameObject, ESwitchableObjectId.E_PANEL);
            _postboxDoor = SelectableObject.GetAsChildByPath(gameObject, ESwitchableObjectId.POSTBOX_LEFT_DOOR);
            _returnableObjects = transform.GetComponentsInChildren<IInitStateReturnable>(true).ToList();
            
            transform.GetComponentsInChildren<InventoryObject>(true)
                .ToList()
                .ForEach(io => _inventoryObjects.Add(io.objectId, io));

            _ePanelDoor.OpenCondition = () => _markStates[EFloorMarkId.RABBIT_SYMBOL];
            _postboxDoor.OpenCondition = () => _markStates[EFloorMarkId.DRAGONFLY];
        }

        private void Awake()
        {
            _textureProjectorPropsGenerator = transform.GetComponentInChildren<TextureProjectorPropsGenerator>();
            _garbagePropsGenerator = transform.GetComponentInChildren<GarbagePropsGenerator>();
            
            _frontWallMaterial = transform.Find(GameConstants.entrywayObjectName).Find(FrontWallName)
                .GetComponent<MeshRenderer>().material;
            _adMaterial = transform.Find(SelectableObject.GetPath(ESwitchableObjectId.AD)).GetComponent<MeshRenderer>()
                .material;

            _ePanelDoorMaterial = transform.Find("e-panel/right_door").GetComponent<MeshRenderer>().material;

            LostRabbitAdTexture = Resources.Load<Texture2D>("Textures/rabbit");
        }

        public void ReturnAllObjectsToInitState(int floorDistanceToPlayer)
        {
            _returnableObjects.ForEach(returnable => returnable.ReturnToInitState(floorDistanceToPlayer));
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
            _frontWallMaterial.SetFloat(FloorNumberPropertyId, number);
        }

        public void EmergeScalpel()
        {
            _scalpel.Emerge();
        }

        public void SetMark(EFloorMarkId id)
        {
            _markStates[id] = true;

            switch (id)
            {
                case EFloorMarkId.DRAGONFLY:
                    _postboxBaseMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 1f);
                    _leftDoor.MarkWithDragonfly();
                    break;

                case EFloorMarkId.RABBIT_AD:
                    _adMaterial.SetTexture(MainTexPropertyId, LostRabbitAdTexture);
                    break;

                case EFloorMarkId.RABBIT_SYMBOL:
                    _ePanelDoorMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 1f);
                    break;
            }
        }

        public void ResetMark(EFloorMarkId id)
        {
            _markStates[id] = false;

            switch (id)
            {
                case EFloorMarkId.DRAGONFLY:
                    _postboxBaseMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 0f);
                    _leftDoor.UnmarkWithDragonfly();
                    break;

                case EFloorMarkId.RABBIT_SYMBOL:
                    _ePanelDoorMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 0f);
                    break;
            }
        }

        public void ResetAllMarks()
        {
            foreach (EFloorMarkId key in _markStates.Keys.ToList()) ResetMark(key);
        }

        public void SetFrontWallRandomAd()
        {
            if (AdGenerator == null) throw new Exception("AdGenerator is not provided to the floor");

            _adMaterial.SetTexture(MainTexPropertyId, AdGenerator.GetRandomAdTexture());
        }

        public void UpdateDoors()
        {
            Transform entrywayTransform = transform.Find(EntrywayObjectName);
            Transform floorLeftDoorBaseTransform = entrywayTransform.Find(LeftDoorBaseObjectName);
            Transform floorRightDoorBaseTransform = entrywayTransform.Find(RightDoorBaseObjectName);

            Door leftDoor = doorController.GenerateRandomDoor();
            Door rightDoor = doorController.GenerateRandomDoor();

            leftDoor.transform.position = floorLeftDoorBaseTransform.position;

            rightDoor.transform.position = floorRightDoorBaseTransform.position;
            rightDoor.Invert();

            leftDoor.name = LeftDoorObjectName;
            rightDoor.name = RightDoorObjectName;

            Transform transformValue = transform;
            leftDoor.transform.SetParent(transformValue);
            rightDoor.transform.SetParent(transformValue);
        }

        public void GenerateRandomTextureProjectorsAndGarbageProps()
        {
            _textureProjectorPropsGenerator.GenerateProps();
            _garbagePropsGenerator.GenerateProps();
        }
    }
}