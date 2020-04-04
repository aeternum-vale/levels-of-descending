using System;
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
    float realSquattingAmount;
    readonly float squattingMaxAmount = 2f;
    readonly float squattingSpeed = 6f;

    readonly float maxDistanceToSelectableObject = .8f;

    SelectableObject selectedObject;
    public GameObject LastGround1Touched { get; private set; }

    bool isStairCommonPace;
    bool isStair1Pace;
    bool prevIsStairPace;
    float stairPaceYTargetOffset;
    float stairPaceYRealOffset;
    float stairPaceXAdjustment;
    bool isGround1Last;
    readonly float stairPaceYSpeed = 0.02f;
    readonly float stairPaceYAmplitude = 0.2f;
    readonly float stairPaceYFrequency = 20f;

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
            UpdateStairPace();
            UpdateSquatting();
            UpdateCameraY();
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
        if (Physics.Raycast(ray, out hit, maxDistanceToSelectableObject))
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
        if (hit.collider.gameObject.name == GameConstants.ground1ColliderObjectName && LastGround1Touched != hit.collider.gameObject)
        {
            LastGround1Touched = hit.collider.gameObject;
            Messenger.Broadcast(Events.FLOOR_TOUCHED);
        }


        prevIsStairPace = isStairCommonPace;

        if (hit.collider.gameObject.name == GameConstants.stairs1ColliderObjectName || hit.collider.gameObject.name == GameConstants.stairs2ColliderObjectName)
        {
            isStairCommonPace = true;
            isStair1Pace = (hit.collider.gameObject.name == GameConstants.stairs1ColliderObjectName);
        }
        else
        {
            if (hit.collider.gameObject.name == GameConstants.ground1ColliderObjectName || hit.collider.gameObject.name == GameConstants.ground2ColliderObjectName)
            {
                isGround1Last = (hit.collider.gameObject.name == GameConstants.ground1ColliderObjectName);
                isStairCommonPace = false;
            }
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

    void UpdateStairPace()
    {
        if (!isStairCommonPace)
        {
            stairPaceYTargetOffset = 0;

            if (stairPaceYRealOffset != stairPaceYTargetOffset)
            {
                float diff = Mathf.Abs(stairPaceYRealOffset - stairPaceYTargetOffset);
                if (diff <= stairPaceYSpeed)
                {
                    stairPaceYRealOffset = stairPaceYTargetOffset;
                }
                else
                {
                    stairPaceYRealOffset += stairPaceYSpeed * (stairPaceYRealOffset < stairPaceYTargetOffset ? 1 : -1) * Time.deltaTime;
                }
            }
        }
        else
        {
            if (prevIsStairPace == false)
            {
                stairPaceXAdjustment = -(gameObject.transform.position.x + (Mathf.PI / 2) + (isStair1Pace ^ isGround1Last ? Mathf.PI : 0));
            }
            stairPaceYRealOffset = (Mathf.Sin((gameObject.transform.position.x + stairPaceXAdjustment) * stairPaceYFrequency) / 2) * stairPaceYAmplitude;
        }
    }

    void UpdateSquatting()
    {
        realSquattingAmount += (Input.GetButton("Squat") ? 1 : -1) * this.squattingSpeed * Time.deltaTime;
        realSquattingAmount = Mathf.Clamp(realSquattingAmount, 0, squattingMaxAmount);
    }

    void UpdateCameraY()
    {
        this.playerCamera.transform.localPosition = new Vector3(0, startCameraY + stairPaceYRealOffset - realSquattingAmount, 0);
    }
}