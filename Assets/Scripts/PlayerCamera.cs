using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	[SerializeField] private Material mat;
	[SerializeField] private Inventory inventory;

	private Camera cameraComponent;
	public float MouseSensitivity { get; } = 1;
	private readonly float mouseVerticalMin = -80;
	private readonly float mouseVerticalMax = 70;
	private float rotationX;

	void Start () {
		cameraComponent = GetComponent<Camera> ();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update () {
		if (!inventory.IsInventoryModeOn)
		{
			rotationX -= Input.GetAxis("Mouse Y") * MouseSensitivity;
			rotationX = Mathf.Clamp(rotationX, mouseVerticalMin, mouseVerticalMax);
			transform.localEulerAngles = new Vector3(rotationX, 0, 0);
		}
	}

	void OnGUI () {
		if (!inventory.IsInventoryModeOn) {
			int size = 8;
			float posX = cameraComponent.pixelWidth / 2 - size / 2;
			float posY = cameraComponent.pixelHeight / 2 - size;
			GUI.Label (new Rect (posX, posY, 80, 80), "◦");
		} else
		{
			inventory.DrawInventory();
		}
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (inventory.IsInventoryModeOn)
		{
			Graphics.Blit(src, dest, mat);
		}
		else
		{
			Graphics.Blit(src, dest);
		}
	}
}