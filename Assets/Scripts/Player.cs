using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] public Inventory inventory;

    Camera playerCameraComponent;

    readonly float speed = 1.5f;
    readonly float gravity = -9.8f;
    float mouseSensitivity;
    CharacterController charController;

    float startCameraY;
    readonly float squattingAmount = 2f;
    readonly float squattingSpeed = 6f;

    SelectableObject selectedObject;
    public GameObject LastFloorTouched { get; private set; }

    void Start()
    {
        playerCameraComponent = playerCamera.GetComponent<Camera>();
        charController = GetComponent<CharacterController>();
        mouseSensitivity = playerCamera.MouseSensitivity;
        startCameraY = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        if (!inventory.IsInventoryModeOn)
        {
            UpdateMouse();
            UpdateMovement();
            UpdateSquatting();
        }
        UpdateUseButton();
        UpdateInventoryButton();
    }

    void UpdateInventoryButton()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (!inventory.IsInventoryModeOn)
            {
                if (inventory.CanActivateInventoryMode)
                {
                    playerCamera.IsInventoryModeOn = true;
                    inventory.ActivateInventoryMode(playerCamera.GetBackgroundTexture());
                    playerCamera.ActivateInventoryMode();
                }
            }
            else
            {
                inventory.OnInventorySwitchToNextItem();
            }
        }
    }
    void UpdateMouse()
    {
        float delta = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotationY = transform.localEulerAngles.y + delta;
        transform.localEulerAngles = new Vector3(0, rotationY, 0);

        if (this.selectedObject)
        {
            this.selectedObject.OnOut();
            this.selectedObject = null;
        }

        Vector3 point = new Vector3(playerCameraComponent.pixelWidth / 2, playerCameraComponent.pixelHeight / 2, 0);
        Ray ray = playerCameraComponent.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.8f))
        {
            SelectableObject currentSelectedObject = (SelectableObject)hit.transform.GetComponent<SelectableObject>();
            if (currentSelectedObject)
            {
                this.selectedObject = currentSelectedObject;
                this.selectedObject.OnOver();
            }
        }
    }

    void UpdateUseButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EInventoryItemID? selectedInventoryItem = null;

            if (inventory.IsInventoryModeOn)
            {
                selectedInventoryItem = inventory.CurrentItemID;
                DeactivateInventoryMode();
            }

            if (this.selectedObject)
            {
                this.selectedObject.OnClick(selectedInventoryItem);
            }
        }

    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.name == "floor" && LastFloorTouched != hit.collider.gameObject)
        {
            LastFloorTouched = hit.collider.gameObject;
            Messenger.Broadcast(Events.FLOOR_TOUCHED);
        }
    }

    void DeactivateInventoryMode()
    {
        playerCamera.DeactivateInventoryMode();
        inventory.DeactivateInventoryMode();
    }

    void UpdateMovement()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);

        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = gravity;
        movement *= Time.deltaTime;

        movement = transform.TransformDirection(movement);
        charController.Move(movement);
    }

    void UpdateSquatting()
    {
        if (Input.GetButton("Squat"))
        {
            this.playerCamera.transform.localPosition += Vector3.down * this.squattingSpeed * Time.deltaTime;
        }
        else
        {
            this.playerCamera.transform.localPosition += Vector3.up * this.squattingSpeed * Time.deltaTime;
        }

        float cameraY = Mathf.Clamp(this.playerCamera.transform.localPosition.y, this.startCameraY - this.squattingAmount, this.startCameraY);

        this.playerCamera.transform.localPosition = new Vector3(
            this.playerCamera.transform.localPosition.x,
            cameraY,
            this.playerCamera.transform.localPosition.z);

    }

}