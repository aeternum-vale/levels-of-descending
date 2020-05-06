﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace SelectableObjectsModule
{
    public class SelectableObject : MonoBehaviour
    {
        [SerializeField] private float maxDistanceToSelect = .45f;
        [SerializeField] private bool hasValueOfMaxDistanceToSelect;

        private readonly Dictionary<GameObject, Material> _materialsCache = new Dictionary<GameObject, Material>();
        private static readonly int IsSelected = Shader.PropertyToID("_IsSelected");

        public bool IsGlowingEnabled { get; set; } = true;

        public float? MaxDistanceToSelect
        {
            get
            {
                if (hasValueOfMaxDistanceToSelect)
                    return maxDistanceToSelect;
                else
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
            if (_materialsCache.ContainsKey(go))
            {
                return _materialsCache[go];
            }
            else
            {
                var mat = go.GetComponent<MeshRenderer>().material;
                _materialsCache.Add(go, mat);
                return mat;
            }
        }

        private void ApplySelectedStateToMaterial(Material mat)
        {
            mat.SetFloat(IsSelected, 1f);
        }

        private void ApplyNormalStateToMaterial(Material mat)
        {
            mat.SetFloat(IsSelected, 0f);
        }
    }
}