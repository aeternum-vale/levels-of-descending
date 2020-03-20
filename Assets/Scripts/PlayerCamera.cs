using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	[SerializeField] private GameObject inventoryCamera;
	[SerializeField] private Material mat;
	[SerializeField] private Inventory inventory;

	private Camera inventoryCameraComponent;

	private Camera cameraComponent;
	private GameObject player;

	private float mouseSensitivity = 1;
	private float mouseVerticalMin = -80;
	private float mouseVerticalMax = 70;
	private float rotationX;

	private RenderTexture rt;

	private Texture2D currentInventoryObjectTexture;
	private RenderTexture nextInventoryObjectTexture;

	void Start () {
		cameraComponent = GetComponent<Camera> ();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		inventoryCameraComponent = inventoryCamera.GetComponent<Camera> ();
		inventoryCameraComponent.targetTexture = new RenderTexture (Screen.width, Screen.height, 24);
	}
	public float MouseSensitivity {
		get { return mouseSensitivity; }
	}

	void Update () {
		rotationX -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
		rotationX = Mathf.Clamp (rotationX, mouseVerticalMin, mouseVerticalMax);
		transform.localEulerAngles = new Vector3 (rotationX, 0, 0);

	}

	void OnGUI () {
		if (!inventory.IsInventoryModeOn) {
			int size = 8;
			float posX = cameraComponent.pixelWidth / 2 - size / 2;
			float posY = cameraComponent.pixelHeight / 2 - size;
			GUI.Label (new Rect (posX, posY, 80, 80), "◦");
		}
	}

	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if (inventory.IsInventoryModeOn) {
			Graphics.Blit (src, dest, mat);

			if (!currentInventoryObjectTexture) {
				inventoryCameraComponent.Render ();
				currentInventoryObjectTexture =  new Texture2D (Screen.width, Screen.height, TextureFormat.ARGB32, false);
				Graphics.CopyTexture(inventoryCameraComponent.targetTexture, currentInventoryObjectTexture);
			}
			
			GL.PushMatrix ();
			GL.LoadPixelMatrix ();
			Graphics.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), currentInventoryObjectTexture, null);
			GL.PopMatrix ();

		} else {
			Graphics.Blit (src, dest);
		}
	}
}