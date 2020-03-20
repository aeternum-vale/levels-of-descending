using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasController : MonoBehaviour {

	[SerializeField] private GameObject playerCamera;
	[SerializeField] private GameObject inventoryCamera;

	private Camera playerCameraComponent;
	private Camera inventoryCameraComponent;

	private AudioListener playerCameraALComponent;
	private AudioListener inventoryCameraALComponent;

	void Awake () {
		playerCameraComponent = playerCamera.GetComponent<Camera> ();
		inventoryCameraComponent = inventoryCamera.GetComponent<Camera> ();

		playerCameraALComponent = playerCamera.GetComponent<AudioListener> ();
		inventoryCameraALComponent = inventoryCamera.GetComponent<AudioListener> ();
	}

	public void setActive (ECameraID cameraID) {
		disableAllCameras ();

		switch (cameraID) {
			case ECameraID.INVENTORY:
				inventoryCamera.tag = "MainCamera";
				inventoryCamera.SetActive (true);
				inventoryCameraALComponent.enabled = true;
				break;
			case ECameraID.PLAYER:
				playerCamera.tag = "MainCamera";
				playerCamera.SetActive (false);
				playerCameraALComponent.enabled = false;
				break;

		}
	}

	protected void disableAllCameras () {
		playerCamera.tag = "Untagged";
		inventoryCamera.tag = "Untagged";
		playerCamera.SetActive (false);
		inventoryCamera.SetActive (false);
		inventoryCameraALComponent.enabled = false;
		playerCameraALComponent.enabled = false;

	}
}