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
		playerCameraComponent = playerCamera.GetComponent<Camera>();
		inventoryCameraComponent = inventoryCamera.GetComponent<Camera>();

		playerCameraALComponent = playerCamera.GetComponent<AudioListener> ();
		inventoryCameraALComponent = inventoryCamera.GetComponent<AudioListener> ();
	}

	public void Activate (ECameraID cameraID) {
		DisableAllCameras();

		switch (cameraID) {
			case ECameraID.INVENTORY:
				inventoryCamera.SetActive (true);
				inventoryCameraALComponent.enabled = true;
				break;
			case ECameraID.PLAYER:
				playerCameraComponent.targetTexture = null;
				playerCamera.SetActive (true);
				playerCameraALComponent.enabled = true;
				break;
		}
	}

	protected void DisableAllCameras () {
		playerCamera.SetActive (false);
		inventoryCamera.SetActive (false);
		inventoryCameraALComponent.enabled = false;
		playerCameraALComponent.enabled = false;
	}

	public void SetInventoryCameraBackroundTexture() {
		playerCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 0);
		playerCameraComponent.Render();
		inventoryCamera.GetComponent<InventoryCamera>().BackgroundTexture = playerCameraComponent.targetTexture;
	}
}