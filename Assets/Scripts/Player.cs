using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	[SerializeField] private PlayerCamera playerCamera;

	private Camera playerCameraComponent;

	private float speed = 1.5f;
	private float gravity = -9.8f;
	private float mouseSensitivity;
	private CharacterController charController;

	private float startCameraY;
	private float squattingAmount = 2f;
	private float squattingSpeed = 0.2f;

	private SelectableObject selectedObject;

	void Start () {
		playerCameraComponent = playerCamera.GetComponent<Camera> ();
		charController = GetComponent<CharacterController> ();
		mouseSensitivity = playerCamera.MouseSensitivity;
		startCameraY = playerCamera.transform.localPosition.y;
	}

	void Update () {
		updateMouse ();
		updateMovement ();
		updateSquatting ();
		updateInventoryButton ();
	}

	void updateInventoryButton () {
		if (Input.GetButtonDown ("Inventory")) {
			Messenger.Broadcast(Events.INVENTORY_BUTTON_PRESSED);
		}
	}

	void updateMouse () {
		float delta = Input.GetAxis ("Mouse X") * mouseSensitivity;
		float rotationY = transform.localEulerAngles.y + delta;
		transform.localEulerAngles = new Vector3 (0, rotationY, 0);

		if (Input.GetMouseButtonDown (0) && this.selectedObject) {
			this.selectedObject.onClick ();
		}

		if (this.selectedObject) {
			this.selectedObject.onOut ();
			this.selectedObject = null;
		}

		Vector3 point = new Vector3 (playerCameraComponent.pixelWidth / 2, playerCameraComponent.pixelHeight / 2, 0);
		Ray ray = playerCameraComponent.ScreenPointToRay (point);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 0.8f)) {
			SelectableObject currentSelectedObject = (SelectableObject) hit.transform.GetComponent<SelectableObject> ();
			if (currentSelectedObject) {
				this.selectedObject = currentSelectedObject;
				this.selectedObject.onOver ();
			}
		}

	}

	void updateMovement () {
		float deltaX = Input.GetAxis ("Horizontal") * speed;
		float deltaZ = Input.GetAxis ("Vertical") * speed;
		Vector3 movement = new Vector3 (deltaX, 0, deltaZ);

		movement = Vector3.ClampMagnitude (movement, speed);
		movement.y = gravity;
		movement *= Time.deltaTime;

		movement = transform.TransformDirection (movement);
		charController.Move (movement);
	}

	void updateSquatting () {
		if (Input.GetButton ("Squat")) {
			this.playerCamera.transform.localPosition += Vector3.down * this.squattingSpeed;
		} else {
			this.playerCamera.transform.localPosition += Vector3.up * this.squattingSpeed;
		}

		float cameraY = Mathf.Clamp (this.playerCamera.transform.localPosition.y, this.startCameraY - this.squattingAmount, this.startCameraY);

		this.playerCamera.transform.localPosition = new Vector3 (
			this.playerCamera.transform.localPosition.x,
			cameraY,
			this.playerCamera.transform.localPosition.z);

	}

}