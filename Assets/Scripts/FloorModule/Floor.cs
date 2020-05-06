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
        private static readonly string FrontWallName = "front_wall";
        private static readonly string FrontWallNumberMatPropertyName = "_FloorNumber";

        private Material _frontWallMaterial;
        private Material _postboxPartMaterialWithDragonFly;
        private Door _leftDoor;
        private Door _rightDoor;
        private Scalpel _scalpel;

        private Material _adMaterial;

        public readonly Dictionary<ESwitchableObjectId, SwitchableObject> SwitchableInstancesDict =
            new Dictionary<ESwitchableObjectId, SwitchableObject>();

        private readonly Dictionary<EFloorMarkId, bool> _markStatesDict = new Dictionary<EFloorMarkId, bool>()
        {
            {EFloorMarkId.DRAGONFLY, false},
            {EFloorMarkId.LOST_PET_SIGN, false}
        };

        private void Start()
        {
            _postboxPartMaterialWithDragonFly =
                transform.Find("postbox").Find("Cube.003").GetComponent<MeshRenderer>().material;
            _leftDoor = gameObject.transform.Find("left_door_prefab").GetComponent<Door>();
            _rightDoor = gameObject.transform.Find("right_door_prefab").GetComponent<Door>();
            _scalpel = transform.Find(InventoryObject.GetPath(EInventoryItemId.SCALPEL)).GetComponent<Scalpel>();

            foreach (var id in (ESwitchableObjectId[]) Enum.GetValues(typeof(ESwitchableObjectId)))
                SwitchableInstancesDict.Add(id,
                    transform.Find(SwitchableObject.GetPath(id)).GetComponent<SwitchableObject>());
        }

        private void Awake()
        {
            _frontWallMaterial = transform.Find(GameConstants.entrywayObjectName).Find(FrontWallName)
                .GetComponent<MeshRenderer>().material;
            _adMaterial = transform.Find(SwitchableObject.GetPath(ESwitchableObjectId.AD)).GetComponent<MeshRenderer>()
                .material;
        }

        public void HideObject(string name)
        {
            transform.Find(name).gameObject.SetActive(false);
        }

        public void ShowObject(string name)
        {
            transform.Find(name).gameObject.SetActive(true);
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
                    _postboxPartMaterialWithDragonFly.SetFloat("_IsTitleOn", 1f);
                    _leftDoor.MarkWithDragonfly();
                    break;

                case EFloorMarkId.LOST_PET_SIGN:
                    _adMaterial.SetFloat("_ActiveTextureNumber", 1f);
                    break;
            }
        }

        public void ResetMark(EFloorMarkId id)
        {
            _markStatesDict[id] = false;

            switch (id)
            {
                case EFloorMarkId.DRAGONFLY:
                    _postboxPartMaterialWithDragonFly.SetFloat("_IsTitleOn", 0f);
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
            _adMaterial.SetTexture("_MainTex", texture);
        }
    }
}