using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class SelectableObject : MonoBehaviour
{
    [SerializeField] float maxDistanceToSelect = .45f;
    [SerializeField] bool hasValueOfMaxDistanceToSelect;

    protected Material selectableMaterial;
    protected Material[] childrenSelectableMaterials;
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
        if (transform.childCount == 0)
        {
            selectableMaterial = GetComponent<MeshRenderer>().material;
        }
        else
        {
            childrenSelectableMaterials = GetComponentsInChildren<MeshRenderer>().Select(m => m.material).ToArray();
        }
    }

    public virtual void OnOver()
    {
        if (IsGlowingEnabled)
        {
            ShowGlowing();
        }
    }

    public virtual void OnOut()
    {
        ShowNormal();
    }

    public virtual void OnClick(EInventoryItemID? selectedInventoryItemId = null)
    {
        //string item = !(selectedInventoryItemId is null) ? selectedInventoryItemId.ToString() : "nothing";
        //Debug.Log($"using {item} on {gameObject.name}");
    }

    public virtual void ShowGlowing()
    {
        if (transform.childCount == 0)
        {
            selectableMaterial.SetFloat("_IsSelected", 1f);
        }
        else
        {
            foreach (var mat in childrenSelectableMaterials)
            {
                mat.SetFloat("_IsSelected", 1f);
            }
        }
    }

    public virtual void ShowNormal()
    {
        if (transform.childCount == 0)
        {
            selectableMaterial.SetFloat("_IsSelected", 0f);
        }
        else
        {
            foreach (var mat in childrenSelectableMaterials)
            {
                mat.SetFloat("_IsSelected", 0f);
            }
        }
    }
}
