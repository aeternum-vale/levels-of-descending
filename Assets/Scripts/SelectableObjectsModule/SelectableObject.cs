﻿using System;
using System.Collections.Generic;
using InventoryModule;
using SelectableObjectsModule.Utilities;
using UnityEngine;
using Utils;

namespace SelectableObjectsModule
{
    public class SelectableObject : MonoBehaviour
    {
        private static readonly int IsSelected = Shader.PropertyToID("_IsSelected");

        private readonly Dictionary<GameObject, Material> _materialsCache = new Dictionary<GameObject, Material>();
        [SerializeField] private bool hasValueOfMaxDistanceToSelect;
        [SerializeField] private float maxDistanceToSelect = .45f;

        public event EventHandler<SelectableObjectClickedEventArgs> Clicked;

        public bool IsGlowingEnabled { get; set; } = true;

        public float? MaxDistanceToSelect
        {
            get
            {
                if (hasValueOfMaxDistanceToSelect)
                    return maxDistanceToSelect;
                return null;
            }
        }

        protected virtual void Awake()
        {
        }

        public virtual void OnOver(GameObject colliderCarrier)
        {
            if (IsGlowingEnabled) ShowGlowing(colliderCarrier);
        }

        public virtual void OnOut()
        {
            ShowNormal();
        }

        public virtual void OnClick(EInventoryItemId? selectedInventoryItemId, GameObject colliderCarrier)
        {
            Clicked?.Invoke(this, new SelectableObjectClickedEventArgs(selectedInventoryItemId, colliderCarrier));
            
            //string item = !(selectedInventoryItemId is null) ? selectedInventoryItemId.ToString() : "nothing";
            //Debug.Log($"using {item} on {gameObject.name}");
        }

        public virtual void ShowGlowing(GameObject colliderCarrier)
        {
            ApplyStateToMaterialRecursively(colliderCarrier, ApplySelectedStateToMaterial);
        }

        public virtual void ShowNormal()
        {
            ApplyStateToMaterialRecursively(gameObject, ApplyNormalStateToMaterial);
        }

        private void ApplyStateToMaterialRecursively(GameObject root, Action<Material> applyStateAction)
        {
            if (root.transform.childCount == 0)
                applyStateAction(GetGameObjectMaterial(root));
            else
                for (var i = 0; i < root.transform.childCount; i++)
                    ApplyStateToMaterialRecursively(root.transform.GetChild(i).gameObject, applyStateAction);
        }

        private Material GetGameObjectMaterial(GameObject go)
        {
            if (_materialsCache.ContainsKey(go)) return _materialsCache[go];

            var mat = go.GetComponent<MeshRenderer>().material;
            _materialsCache.Add(go, mat);
            return mat;
        }

        private static void ApplySelectedStateToMaterial(Material mat)
        {
            mat.SetFloat(IsSelected, 1f);
        }

        private static void ApplyNormalStateToMaterial(Material mat)
        {
            mat.SetFloat(IsSelected, 0f);
        }

        public static string GetPath(ESwitchableObjectId id) =>
            GameConstants.switchableObjectToInstancePathMap[id];

        public static string GetName(ESwitchableObjectId id) =>
            GameUtils.GetNameByPath(GameConstants.switchableObjectToInstancePathMap[id]);

        public static string GetPath(EInventoryItemId id) =>
            GameConstants.inventoryItemToInstancePathMap[id];

        public static string GetName(EInventoryItemId id) =>
            GameUtils.GetNameByPath(GameConstants.inventoryItemToInstancePathMap[id]);

        public static SwitchableObject GetAsChild(GameObject parent, ESwitchableObjectId id) =>
            parent.transform.Find(GetName(id)).GetComponent<SwitchableObject>();

        public static InventoryObject GetAsChild(GameObject parent, EInventoryItemId id) =>
            parent.transform.Find(GetName(id)).GetComponent<InventoryObject>();

        public static T GetAsChild<T>(GameObject parent, string objName) where T : SelectableObject =>
            parent.transform.Find(objName).GetComponent<T>();
    }
}