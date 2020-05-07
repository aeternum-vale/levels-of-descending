using System;
using System.Collections.Generic;
using System.Linq;
using DoorModule;
using InventoryModule;
using SelectableObjectsModule;
using SelectableObjectsModule.SpecificObjects;
using UnityEngine;

namespace FloorModule
{
    public class Floor : MonoBehaviour
    {
        private const string FrontWallName = "front_wall";
        private const string FrontWallNumberMatPropertyName = "_FloorNumber";

        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int IsTitleOn = Shader.PropertyToID("_IsTitleOn");
        private static readonly int ActiveTextureNumber = Shader.PropertyToID("_ActiveTextureNumber");

        private readonly Dictionary<EFloorMarkId, bool> _markStatesDict = new Dictionary<EFloorMarkId, bool>
        {
            {EFloorMarkId.DRAGONFLY, false},
            {EFloorMarkId.LOST_PET_SIGN, false}
        };

        public readonly Dictionary<ESwitchableObjectId, SwitchableObject> SwitchableInstancesDict =
            new Dictionary<ESwitchableObjectId, SwitchableObject>();

        private Material _adMaterial;

        private Material _frontWallMaterial;
        private Door _leftDoor;
        private Material _postboxPartMaterialWithDragonFly;
        private Door _rightDoor;
        private Scalpel _scalpel;

        private void Start()
        {
            _postboxPartMaterialWithDragonFly =
                transform.Find("postbox").Find("Cube.003").GetComponent<MeshRenderer>().material;
            _leftDoor = gameObject.transform.Find("left_door_prefab").GetComponent<Door>();
            _rightDoor = gameObject.transform.Find("right_door_prefab").GetComponent<Door>();
            _scalpel = transform.Find(SelectableObject.GetPath(EInventoryItemId.SCALPEL)).GetComponent<Scalpel>();

            // foreach (var id in (ESwitchableObjectId[]) Enum.GetValues(typeof(ESwitchableObjectId)))
            //     SwitchableInstancesDict.Add(id,
            //         transform.Find(SelectableObject.GetPath(id)).GetComponent<SwitchableObject>());
        }

        private void Awake()
        {
            _frontWallMaterial = transform.Find(GameConstants.entrywayObjectName).Find(FrontWallName)
                .GetComponent<MeshRenderer>().material;
            _adMaterial = transform.Find(SelectableObject.GetPath(ESwitchableObjectId.AD)).GetComponent<MeshRenderer>()
                .material;
        }

        public void HideObject(string objectName)
        {
            transform.Find(objectName).gameObject.SetActive(false);
        }

        public void ShowObject(string objectName)
        {
            transform.Find(objectName).gameObject.SetActive(true);
        }

        public void SetFloorDrawnNumber(int number)
        {
            _frontWallMaterial.SetFloat(FrontWallNumberMatPropertyName, number);
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
    }
}