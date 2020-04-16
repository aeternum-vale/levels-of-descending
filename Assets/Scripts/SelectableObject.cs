using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]

public class SelectableObject : MonoBehaviour
{

    protected Material selectableMaterial;

    void Awake()
    {
        selectableMaterial = GetComponent<MeshRenderer>().material;
    }

    public virtual void OnOver()
    {
        ShowSelected();
    }

    public virtual void OnOut()
    {
        ShowNormal();
    }

    public virtual void OnClick(EInventoryItemID? selectedInventoryItemId = null)
    {
        string item = !(selectedInventoryItemId is null) ? selectedInventoryItemId.ToString() : "nothing";
        //Debug.Log($"using {item} on {gameObject.name}");
    }

    public virtual void ShowSelected()
    {
        selectableMaterial.SetFloat("_IsSelected", 1f);
    }

    public virtual void ShowNormal()
    {
        selectableMaterial.SetFloat("_IsSelected", 0f);
    }
}
