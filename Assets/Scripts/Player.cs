using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] public Inventory inventory;
    [SerializeField] public bool isSquattingOn = true;

    Camera playerCameraComponent;

    readonly float speed = 1.5f;
    readonly float gravity = -9.8f;
    float mouseSensitivity;
    CharacterController charController;

    float startCameraY;
    float realSquattingAmount;
    readonly float squattingMaxAmount = 2f;
    readonly float squattingSpeed = 3f;

    float maxDistanceToSelectableObject = .45f;
    static readonly float maxDistanceToSelectableObjectOnStanding = .45f;
    static readonly float maxDistanceToSelectableObjectOnSquatting = .8f;

    SelectableObject selectedObject;
    public GameObject LastGround1ColliderTouched { get; private set; }
    public GameObject PrevLastGround1ColliderTouched { get; private set; }

    bool isStairCommonPace;
    bool isStair1Pace;
    bool prevIsStairPace;
    float stairPaceYTargetOffset;
    float stairPaceYRealOffset;
    float stairPaceXAdjustment;
    bool isGround1Last;
    static readonly float stairPaceYSpeed = 0.02f;
    static readonly float stairPaceYAmplitude = 0.2f;
    static readonly float stairPaceYFrequency = 20f;

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
        if (Physics.Raycast(ray, out hit))
        {

            Transform currentTransform = hit.transform;
            SelectableObject currentSelectedObject = currentTransform.GetComponent<SelectableObject>();

            while (currentSelectedObject == null && currentTransform.parent != null)
            {
                currentTransform = currentTransform.parent;
                currentSelectedObject = currentTransform.GetComponent<SelectableObject>();
            }

            if (currentSelectedObject)
            {
                float distance = maxDistanceToSelectableObject;
                if (currentSelectedObject.MaxDistanceToSelect != null)
                {
                    distance = (float)currentSelectedObject.MaxDistanceToSelect;
                }

                if (hit.distance <= distance)
                {
                    this.selectedObject = currentSelectedObject;
                    this.selectedObject.OnOver(hit.transform.gameObject);
                }
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
        if (hit.collider.gameObject.name == GameConstants.ground1ColliderObjectName && LastGround1ColliderTouched != hit.collider.gameObject)
        {
            PrevLastGround1ColliderTouched = LastGround1ColliderTouched;
            LastGround1ColliderTouched = hit.collider.gameObject;
            Messenger.Broadcast(Events.FLOOR_WAS_TOUCHED);
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
                stairPaceXAdjustment = -(gameObject.transform.position.x + (!isStair1Pace ? Mathf.PI : 0));
            }
            stairPaceYRealOffset = (Mathf.Sin((gameObject.transform.position.x + stairPaceXAdjustment) * stairPaceYFrequency) / 2) * stairPaceYAmplitude;
        }
    }

    void UpdateSquatting()
    {
        if (isSquattingOn)
        {
            realSquattingAmount += (Input.GetButton("Squat") ? 1 : -1) * this.squattingSpeed * Time.deltaTime;
            realSquattingAmount = Mathf.Clamp(realSquattingAmount, 0, squattingMaxAmount);

            if (realSquattingAmount == squattingMaxAmount)
            {
                maxDistanceToSelectableObject = maxDistanceToSelectableObjectOnSquatting;
            }
            else
            {
                maxDistanceToSelectableObject = maxDistanceToSelectableObjectOnStanding;
            }
        }
    }

    void UpdateCameraY()
    {
        this.playerCamera.transform.localPosition = new Vector3(0, startCameraY + stairPaceYRealOffset - realSquattingAmount, 0);
    }
}