using InventoryModule;
using Plugins;
using SelectableObjectsModule;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerModule
{
	public class Player : MonoBehaviour
	{
		private static readonly float MaxDistanceToSelectableObjectOnStanding = .45f;
		private static readonly float MaxDistanceToSelectableObjectOnSquatting = .8f;
		private static readonly float StairPaceYSpeed = 0.02f;
		private static readonly float StairPaceYAmplitude = 0.2f;
		private static readonly float StairPaceYFrequency = 20f;
		private readonly float _cutSceneMoveDurationSec = 4f;
		private readonly float _gravity = -9.8f;

		private readonly float _squattingMaxAmount = 2f;
		private readonly float _squattingSpeed = 3f;
		private readonly float _mouseVerticalMax = 70;
		private readonly float _mouseVerticalMin = -80;


		private GameObject _bindingPlatform;
		private float _bindingPlatformStartY;
		private CharacterController _charController;
		private GameObject _colliderCarrier;

		private bool _isCutSceneMoving;
		private bool _isGround1Last;
		private bool _isStair1Pace;

		private bool _isStairCommonPace;
		private bool _isYBind;

		private float _maxDistanceToSelectableObject = .45f;

		private Camera _playerCameraComponent;
		private Transform _playerTransform;
		private float _playerYBeforeBinding;
		private bool _prevIsStairPace;
		private float _realSquattingAmount;

		private SelectableObject _selectedObject;
		private float _stairPaceXAdjustment;
		private float _stairPaceYRealOffset;
		private float _stairPaceYTargetOffset;

		private float _startCameraY;
		private bool _isSquattingOn = false;



		[SerializeField] private Inventory _inventory;
		[SerializeField] private PlayerCamera _playerCamera;
		[SerializeField] private InputsCombiner _input;

		[Space(10)]

		[SerializeField] private float _mouseSensitivity = 0.05f;
		[SerializeField] private float _movementSpeed = 1.5f;

		public GameObject LastGround1ColliderTouched { get; private set; }
		public GameObject PrevLastGround1ColliderTouched { get; private set; }


		private void Start()
		{
			_playerCameraComponent = _playerCamera.GetComponent<Camera>();
			_charController = GetComponent<CharacterController>();
			_startCameraY = _playerCamera.transform.localPosition.y;
			_playerTransform = transform;

		}

		private void Update()
		{
			if (!_inventory.IsInventoryModeOn)
			{
				if (!_isCutSceneMoving)
				{
					UpdateMouse();
					UpdateCameraY();
					UpdateMovement();
					UpdateStairPace();
					UpdateSquatting();
				}

				if (_isYBind) UpdateBindY();
			}

			UpdateUseButton();
			UpdateInventoryButton();
			UpdateExitButton();
		}

		private void UpdateInventoryButton()
		{
			if (!_input.inventoryDown) return;

			if (_playerCamera.IsCutSceneMoving) return;

			if (!_inventory.IsInventoryModeOn)
			{
				if (!_inventory.CanActivateInventoryMode) return;

				Messenger.Broadcast(Events.InventoryModeBeforeActivating);

				_playerCamera.IsInventoryModeOn = true;
				_inventory.ActivateInventoryMode(_playerCamera.GetBackgroundTexture());
				_playerCamera.ActivateInventoryMode();
			}
			else
			{
				_inventory.OnInventorySwitchToNextItem();
			}


		}

		private void UpdateMouse()
		{
			float deltaY = _input.look.x * _mouseSensitivity;
			Transform transformValue = transform;
			float rotationY = transformValue.localEulerAngles.y + deltaY;
			transformValue.localEulerAngles = new Vector3(0, rotationY, 0);

			float deltaX = _input.look.y * _mouseSensitivity;
			Transform cameraTransformValue = _playerCamera.transform;
			float rotationX = cameraTransformValue.localEulerAngles.x + deltaX;
			rotationX = NormalizeAngle(rotationX, -90, 90);
			rotationX = Mathf.Clamp(rotationX, _mouseVerticalMin, _mouseVerticalMax);
			cameraTransformValue.localEulerAngles = new Vector3(rotationX, 0, 0);

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
				distance = (float)currentSelectedObject.MaxDistanceToSelect;

			if (!(hit.distance <= distance)) return;

			_selectedObject = currentSelectedObject;
			_colliderCarrier = hit.transform.gameObject;
			_selectedObject.OnOver(_colliderCarrier);
		}

		private void UpdateUseButton()
		{
			if (!_input.useDown) return;

			EInventoryItemId? selectedInventoryItem = null;

			if (_inventory.IsInventoryModeOn)
			{
				selectedInventoryItem = _inventory.CurrentItemId;
				DeactivateInventoryMode();

				if (!_selectedObject)
					Messenger.Broadcast(Events.InventoryItemUsedIncorrectly);
			}

			if (_selectedObject)
				_selectedObject.OnClick(selectedInventoryItem, _colliderCarrier);
		}

		private void UpdateExitButton()
		{
			//if (!Input.GetButtonDown("Cancel")) return; TODO
			return;

			if (_inventory.IsInventoryModeOn)
				DeactivateInventoryMode();
			else
				Messenger.Broadcast(Events.ExitButtonClicked);
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
			_playerCamera.DeactivateInventoryMode();
			_inventory.DeactivateInventoryMode();
		}

		private void UpdateMovement()
		{
			float movementSpeed = _movementSpeed * (_isSquattingOn ? 0.4f : 1f);

			float deltaX = _input.move.x * movementSpeed;
			float deltaZ = _input.move.y * movementSpeed;

			Vector3 movement = new Vector3(deltaX, 0, deltaZ);

			movement = Vector3.ClampMagnitude(movement, movementSpeed);
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
			if (_input.squatDown)
				_isSquattingOn = !_isSquattingOn;

			_realSquattingAmount += (_isSquattingOn ? 1 : -1) * _squattingSpeed * Time.deltaTime;
			_realSquattingAmount = Mathf.Clamp(_realSquattingAmount, 0, _squattingMaxAmount);

			_maxDistanceToSelectableObject = _realSquattingAmount == _squattingMaxAmount
				? MaxDistanceToSelectableObjectOnSquatting
				: MaxDistanceToSelectableObjectOnStanding;
		}

		private void UpdateCameraY()
		{
			_playerCamera.transform.localPosition =
				new Vector3(0, _startCameraY + _stairPaceYRealOffset - _realSquattingAmount, 0);
		}

		public void CutSceneMoveToPosition(Vector3 position, Vector3 rotation, Vector3 cameraRotation)
		{
			_isCutSceneMoving = true;
			_playerCamera.IsCutSceneMoving = true;

			iTween.MoveTo(gameObject,
				iTween.Hash("position", position,
					"time", _cutSceneMoveDurationSec,
					"oncomplete", "OnCutSceneMoveComplete"));
			iTween.RotateTo(gameObject, rotation, _cutSceneMoveDurationSec);
			iTween.RotateTo(_playerCamera.gameObject, cameraRotation, _cutSceneMoveDurationSec);
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


		private static float NormalizeAngle(float value, int start, int end)
		{
			int width = end - start;
			float offsetValue = value - start;

			return (float)(offsetValue + start - (int)(offsetValue / width) * width);
		}

	}
}