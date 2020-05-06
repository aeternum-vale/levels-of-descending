using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] float maxDistanceToSelect = .45f;
    [SerializeField] bool hasValueOfMaxDistanceToSelect;

    Dictionary<GameObject, Material> materialsCache = new Dictionary<GameObject, Material>();

    public bool IsGlowingEnabled { get; set; } = true;

    public float? MaxDistanceToSelect
    {
        get
        {
            if (hasValueOfMaxDistanceToSelect)
            {
                return maxDistanceToSelect;
            } else
            {
                return null;
            }
        }
    }

    protected virtual void Awake()
    {

    }

    public virtual void OnOver(GameObject colliderCarrier)
    {
        if (IsGlowingEnabled)
        {
            ShowGlowing(colliderCarrier);
        }
    }

    public virtual void OnOut()
    {
        ShowNormal();
    }

    public virtual void OnClick(EInventoryItemID? selectedInventoryItemId, GameObject colliderCarrier)
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

    void ApplyStateToMaterialRecursively(GameObject root, Action<Material> applyStateAction)
    {
        if (root.transform.childCount == 0)
        {
            applyStateAction(GetGameObjectMaterial(root));
        } else
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                ApplyStateToMaterialRecursively(root.transform.GetChild(i).gameObject, applyStateAction);
            }
        }
    }

    Material GetGameObjectMaterial(GameObject go)
    {
        if (materialsCache.ContainsKey(go))
        {
            return materialsCache[go];
        } else
        {
            Material mat = go.GetComponent<MeshRenderer>().material;
            materialsCache.Add(go, mat);
            return mat;
        }
    }

    void ApplySelectedStateToMaterial(Material mat)
    {
        mat.SetFloat("_IsSelected", 1f);
    }

    void ApplyNormalStateToMaterial(Material mat)
    {
        mat.SetFloat("_IsSelected", 0f);
    }
}
