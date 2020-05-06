using InventoryModule;
using Plugins;
using SelectableObjectsModule;
using UnityEngine;

namespace PlayerModule
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] public Inventory inventory;
        [SerializeField] public bool isSquattingOn = true;

        private Camera _playerCameraComponent;

        private readonly float _speed = 1.5f;
        private readonly float _gravity = -9.8f;
        private float _mouseSensitivity;
        private CharacterController _charController;

        private float _startCameraY;
        private float _realSquattingAmount;
        private readonly float _squattingMaxAmount = 2f;
        private readonly float _squattingSpeed = 3f;

        private float _maxDistanceToSelectableObject = .45f;
        private static readonly float MaxDistanceToSelectableObjectOnStanding = .45f;
        private static readonly float MaxDistanceToSelectableObjectOnSquatting = .8f;

        private SelectableObject _selectedObject;
        private GameObject _colliderCarrier;
        public GameObject LastGround1ColliderTouched { get; private set; }
        public GameObject PrevLastGround1ColliderTouched { get; private set; }

        private bool _isStairCommonPace;
        private bool _isStair1Pace;
        private bool _prevIsStairPace;
        private float _stairPaceYTargetOffset;
        private float _stairPaceYRealOffset;
        private float _stairPaceXAdjustment;
        private bool _isGround1Last;
        private static readonly float StairPaceYSpeed = 0.02f;
        private static readonly float StairPaceYAmplitude = 0.2f;
        private static readonly float StairPaceYFrequency = 20f;

        private void Start()
        {
            _playerCameraComponent = playerCamera.GetComponent<Camera>();
            _charController = GetComponent<CharacterController>();
            _mouseSensitivity = playerCamera.MouseSensitivity;
            _startCameraY = playerCamera.transform.localPosition.y;
        }

        private void Update()
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

        private void UpdateInventoryButton()
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

        private void UpdateMouse()
        {
            var delta = Input.GetAxis("Mouse X") * _mouseSensitivity;
            var transformValue = transform;

            var rotationY = transformValue.localEulerAngles.y + delta;
            transformValue.localEulerAngles = new Vector3(0, rotationY, 0);

            if (_selectedObject)
            {
                _selectedObject.OnOut();
                _selectedObject = null;
            }

            var point = new Vector3(_playerCameraComponent.pixelWidth / 2, _playerCameraComponent.pixelHeight / 2, 0);
            var ray = _playerCameraComponent.ScreenPointToRay(point);

            if (!Physics.Raycast(ray, out var hit)) return;
            
            var currentTransform = hit.transform;
            var currentSelectedObject = currentTransform.GetComponent<SelectableObject>();

            while (currentSelectedObject == null && currentTransform.parent != null)
            {
                currentTransform = currentTransform.parent;
                currentSelectedObject = currentTransform.GetComponent<SelectableObject>();
            }

            if (!currentSelectedObject) return;
            
            var distance = _maxDistanceToSelectableObject;
            if (currentSelectedObject.MaxDistanceToSelect != null)
                distance = (float) currentSelectedObject.MaxDistanceToSelect;

            if (!(hit.distance <= distance)) return;
            
            _selectedObject = currentSelectedObject;
            _colliderCarrier = hit.transform.gameObject;
            _selectedObject.OnOver(_colliderCarrier);
        }

        private void UpdateUseButton()
        {
            if (Input.GetMouseButtonDown(0))
            {
                EInventoryItemId? selectedInventoryItem = null;

                if (inventory.IsInventoryModeOn)
                {
                    selectedInventoryItem = inventory.CurrentItemId;
                    DeactivateInventoryMode();
                }

                if (_selectedObject) _selectedObject.OnClick(selectedInventoryItem, _colliderCarrier);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.gameObject.name == GameConstants.ground1ColliderObjectName &&
                LastGround1ColliderTouched != hit.collider.gameObject)
            {
                PrevLastGround1ColliderTouched = LastGround1ColliderTouched;
                LastGround1ColliderTouched = hit.collider.gameObject;
                Messenger.Broadcast(Events.floorWasTouched);
            }

            _prevIsStairPace = _isStairCommonPace;

            if (hit.collider.gameObject.name == GameConstants.stairs1ColliderObjectName ||
                hit.collider.gameObject.name == GameConstants.stairs2ColliderObjectName)
            {
                _isStairCommonPace = true;
                _isStair1Pace = hit.collider.gameObject.name == GameConstants.stairs1ColliderObjectName;
            }
            else
            {
                if (hit.collider.gameObject.name == GameConstants.ground1ColliderObjectName ||
                    hit.collider.gameObject.name == GameConstants.ground2ColliderObjectName)
                {
                    _isGround1Last = hit.collider.gameObject.name == GameConstants.ground1ColliderObjectName;
                    _isStairCommonPace = false;
                }
            }
        }

        private void DeactivateInventoryMode()
        {
            playerCamera.DeactivateInventoryMode();
            inventory.DeactivateInventoryMode();
        }

        private void UpdateMovement()
        {
            var deltaX = Input.GetAxis("Horizontal") * _speed;
            var deltaZ = Input.GetAxis("Vertical") * _speed;
            var movement = new Vector3(deltaX, 0, deltaZ);

            movement = Vector3.ClampMagnitude(movement, _speed);
            movement.y = _gravity;
            movement *= Time.deltaTime;

            movement = transform.TransformDirection(movement);
            _charController.Move(movement);
        }

        private void UpdateStairPace()
        {
            if (!_isStairCommonPace)
            {
                _stairPaceYTargetOffset = 0;

                if (_stairPaceYRealOffset == _stairPaceYTargetOffset) return;
                
                var diff = Mathf.Abs(_stairPaceYRealOffset - _stairPaceYTargetOffset);
                if (diff <= StairPaceYSpeed)
                    _stairPaceYRealOffset = _stairPaceYTargetOffset;
                else
                    _stairPaceYRealOffset +=
                        StairPaceYSpeed * (_stairPaceYRealOffset < _stairPaceYTargetOffset ? 1 : -1) *
                        Time.deltaTime;
            }
            else
            {
                if (_prevIsStairPace == false)
                    _stairPaceXAdjustment = -(gameObject.transform.position.x + (!_isStair1Pace ? Mathf.PI : 0));
                _stairPaceYRealOffset =
                    Mathf.Sin((gameObject.transform.position.x + _stairPaceXAdjustment) * StairPaceYFrequency) / 2 *
                    StairPaceYAmplitude;
            }
        }

        private void UpdateSquatting()
        {
            if (!isSquattingOn) return;
            
            _realSquattingAmount += (Input.GetButton("Squat") ? 1 : -1) * _squattingSpeed * Time.deltaTime;
            _realSquattingAmount = Mathf.Clamp(_realSquattingAmount, 0, _squattingMaxAmount);

            _maxDistanceToSelectableObject = (_realSquattingAmount == _squattingMaxAmount)
                ? MaxDistanceToSelectableObjectOnSquatting
                : MaxDistanceToSelectableObjectOnStanding;
        }

        private void UpdateCameraY()
        {
            playerCamera.transform.localPosition =
                new Vector3(0, _startCameraY + _stairPaceYRealOffset - _realSquattingAmount, 0);
        }
    }
}