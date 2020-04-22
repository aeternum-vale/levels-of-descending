using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]

public class SelectableObject : MonoBehaviour
{

    protected Material selectableMaterial;
    protected Material[] childrenSelectableMaterials;
    public bool IsEnabled { get; set; } = true;

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
        if (IsEnabled)
        {
            ShowSelected();
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

    public virtual void ShowSelected()
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
