using UnityEngine;

[RequireComponent (typeof (Collider))]
[RequireComponent (typeof (MeshRenderer))]

public class SelectableObject : MonoBehaviour {

	protected Material selectableMaterial;

	void Awake() {
		selectableMaterial = GetComponent <MeshRenderer>().material;
	}

	public virtual void OnOver() {
		ShowSelected();
	}

	public virtual void OnOut() {
		ShowNormal();
	}

	public virtual void OnClick(EInventoryItemID? selectedInventoryItemId = null) {
	}

	public virtual void ShowSelected() {
		selectableMaterial.SetFloat("_IsSelected", 1f);
	}

	public virtual void ShowNormal() {
		selectableMaterial.SetFloat("_IsSelected", 0f);
	}
}
