using UnityEngine;

[RequireComponent (typeof (Collider))]
[RequireComponent (typeof (MeshRenderer))]

public class SelectableObject : MonoBehaviour {

	protected Material selectableMaterial;

	void Awake() {
		selectableMaterial = GetComponent <MeshRenderer>().material;
	}

	public virtual void onOver() {
		showSelected();
	}

	public virtual void onOut() {
		showNormal();
	}

	public virtual void onClick() {
		
	}

	public virtual void showSelected() {
		selectableMaterial.SetFloat("_IsSelected", 1f);
	}

	public virtual void showNormal() {
		selectableMaterial.SetFloat("_IsSelected", 0f);
	}
}
