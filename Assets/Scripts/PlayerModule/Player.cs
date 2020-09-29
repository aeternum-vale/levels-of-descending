using System;
using InventoryModule;
using JetBrains.Annotations;
using Plugins;
using SelectableObjectsModule;
using UnityEngine;

namespace PlayerModule
{
    public class Player : MonoBehaviour
    {
        private static readonly float MaxDistanceToSelectableObjectOnStanding = .45f;
        private static readonly float MaxDistanceToSelectableObjectOnSquatting = .8f;
        private static readonly float StairPaceYSpeed = 0.02f;
        private static readonly float StairPaceYAmplitude = 0.2f;
        private static readonly float StairPaceYFrequency = 20f;
        private readonly float _gravity = -9.8f;

        private readonly float _speed = 1.5f;
        private readonly float _squattingMaxAmount = 2f;
        private readonly float _squattingSpeed = 3f;
        private CharacterController _charController;
        private GameObject _colliderCarrier;
        private bool _isGround1Last;
        private bool _isStair1Pace;

        private bool _isStairCommonPace;

        private float _maxDistanceToSelectableObject = .45f;
        private float _mouseSensitivity;

        private Camera _playerCameraComponent;
        private bool _prevIsStairPace;
        private float _realSquattingAmount;

        private SelectableObject _selectedObject;
        private float _stairPaceXAdjustment;
        private float _stairPaceYRealOffset;
        private float _stairPaceYTargetOffset;

        private float _startCameraY;
        [SerializeField] public Inventory inventory;
        [SerializeField] public bool isSquattingOn = true;
        [SerializeField] private PlayerCamera playerCamera;
        public GameObject LastGround1ColliderTouched { get; private set; }
        public GameObject PrevLastGround1ColliderTouched { get; private set; }

        private bool _isCutSceneMoving = false;
        private bool _isYBind = false;
        private Transform _playerTransform;

        private GameObject _bindingPlatform;
        private float _bindingPlatformStartY;
        private float _playerYBeforeBinding;

        private void Start()
        {
            _playerCameraComponent = playerCamera.GetComponent<Camera>();
            _charController = GetComponent<CharacterController>();
            _mouseSensitivity = playerCamera.MouseSensitivity;
            _startCameraY = playerCamera.transform.localPosition.y;
            _playerTransform = transform;
        }

        private void Update()
        {
            if (!inventory.IsInventoryModeOn)
            {
                if (!_isCutSceneMoving)
                {
                    UpdateMouse();
                    UpdateCameraY();
                    UpdateMovement();
                    UpdateStairPace();
                    UpdateSquatting();
                }
                

                if (_isYBind)
                {
                    UpdateBindY();
                }
            }

            UpdateUseButton();
            UpdateInventoryButton();
        }

        private void UpdateInventoryButton()
        {
            if (!Input.GetButtonDown("Inventory")) return;

            if (!inventory.IsInventoryModeOn)
            {
                if (!inventory.CanActivateInventoryMode) return;

                playerCamera.IsInventoryModeOn = true;
                inventory.ActivateInventoryMode(playerCamera.GetBackgroundTexture());
                playerCamera.ActivateInventoryMode();
            }
            else
            {
                inventory.OnInventorySwitchToNextItem();
            }
        }

        private void UpdateMouse()
        {
            float delta = Input.GetAxis("Mouse X") * _mouseSensitivity;
            Transform transformValue = transform;

            float rotationY = transformValue.localEulerAngles.y + delta;
            transformValue.localEulerAngles = new Vector3(0, rotationY, 0);

            if (_selectedObject)
            {
                _selectedObject.OnOut();
                _selectedObject = null;
            }

            Vector3 point = new Vector3(_playerCameraComponent.pixelWidth / 2, _playerCameraComponent.pixelHeight / 2,
                0);
            Ray ray = _playerCameraComponent.ScreenPointToRay(point);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            Transform currentTransform = hit.transform;
            SelectableObject currentSelectedObject = currentTransform.GetComponent<SelectableObject>();

            while (currentSelectedObject == null && currentTransform.parent != null)
            {
                currentTransform = currentTransform.parent;
                currentSelectedObject = currentTransform.GetComponent<SelectableObject>();
            }

            if (!currentSelectedObject) return;

            float distance = _maxDistanceToSelectableObject;
            if (currentSelectedObject.MaxDistanceToSelect != null)
                distance = (float) currentSelectedObject.MaxDistanceToSelect;

            if (!(hit.distance <= distance)) return;

            _selectedObject = currentSelectedObject;
            _colliderCarrier = hit.transform.gameObject;
            _selectedObject.OnOver(_colliderCarrier);
        }

        private void UpdateUseButton()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            EInventoryItemId? selectedInventoryItem = null;

            if (inventory.IsInventoryModeOn)
            {
                selectedInventoryItem = inventory.CurrentItemId;
                DeactivateInventoryMode();
            }

            if (_selectedObject)
                _selectedObject.OnClick(selectedInventoryItem, _colliderCarrier);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!_isCutSceneMoving && hit.collider.gameObject.name == GameConstants.elevatorFloorColliderObjectName)
            {
                Messenger.Broadcast(Events.ElevatorFloorWasTouched);
                return;
            }

            if (hit.collider.gameObject.name == GameConstants.ground1ColliderObjectName &&
                LastGround1ColliderTouched != hit.collider.gameObject)
            {
                PrevLastGround1ColliderTouched = LastGround1ColliderTouched;
                LastGround1ColliderTouched = hit.collider.gameObject;
                Messenger.Broadcast(Events.FloorWasTouched);
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
            float deltaX = Input.GetAxis("Horizontal") * _speed;
            float deltaZ = Input.GetAxis("Vertical") * _speed;
            Vector3 movement = new Vector3(deltaX, 0, deltaZ);

            movement = Vector3.ClampMagnitude(movement, _speed);
            movement.y = _gravity;
            movement *= Time.deltaTime;

            movement = transform.TransformDirection(movement);
            _charController.Move(movement);
        }

        private void UpdateStairPace()
        {
            if (_isCutSceneMoving) return;

            if (!_isStairCommonPace)
            {
                _stairPaceYTargetOffset = 0;

                if (_stairPaceYRealOffset == _stairPaceYTargetOffset) return;

                float diff = Mathf.Abs(_stairPaceYRealOffset - _stairPaceYTargetOffset);
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

            _maxDistanceToSelectableObject = _realSquattingAmount == _squattingMaxAmount
                ? MaxDistanceToSelectableObjectOnSquatting
                : MaxDistanceToSelectableObjectOnStanding;
        }

        private void UpdateCameraY()
        {
            playerCamera.transform.localPosition =
                new Vector3(0, _startCameraY + _stairPaceYRealOffset - _realSquattingAmount, 0);
        }

        public void CutSceneMoveToPosition(Vector3 position, Vector3 rotation, Vector3 cameraRotation)
        {
            _isCutSceneMoving = true;
            playerCamera.IsCutSceneMoving = true;

            iTween.MoveTo(gameObject,
                iTween.Hash("position", position, "time", 2, "oncomplete", "OnCutSceneMoveComplete"));
            iTween.RotateTo(gameObject, rotation, 2);
            iTween.RotateTo(playerCamera.gameObject, cameraRotation, 2);
        }

        public void OnCutSceneMoveComplete()
        {
            Messenger.Broadcast(Events.PlayerCutSceneMoveCompleted);
        }

        public void BindYTo(GameObject platform)
        {
            _isYBind = true;
            _bindingPlatform = platform;
            _bindingPlatformStartY = platform.transform.position.y;
            _playerYBeforeBinding = transform.position.y;
        }

        private void UpdateBindY()
        {
            Vector3 pos = _playerTransform.position;
            
            _playerTransform.position = new Vector3(
                pos.x,
                _playerYBeforeBinding + (_bindingPlatform.transform.position.y - _bindingPlatformStartY),
                pos.z
            );
        }
    }
}