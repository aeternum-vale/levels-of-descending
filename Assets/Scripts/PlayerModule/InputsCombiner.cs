using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace PlayerModule
{
	public class InputsCombiner : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;

		public bool use;
		public bool squat;
		public bool inventory;
		
		public bool useDown;
		public bool squatDown;
		public bool inventoryDown;

		private bool _usePrev;
		private bool _squatPrev;
		private bool _inventoryPrev;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnUse(InputValue value)
		{
			UseInput(value.isPressed);
		}

		public void OnSquat(InputValue value)
		{
			SquatInput(value.isPressed);
		}

		public void OnInventory(InputValue value)
		{
			InventoryInput(value.isPressed);
		}
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}


		public void UseInput(bool newUseState)
		{
			use = newUseState;
		}

		public void SquatInput(bool newSquatState)
		{
			squat = newSquatState;
		}

		public void InventoryInput(bool newInventoryInput)
		{
			inventory = newInventoryInput;
		}



#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

		private void Update()
		{
			useDown = use && !_usePrev;
			squatDown = squat && !_squatPrev;
			inventoryDown = inventory && !_inventoryPrev;

			_usePrev = use;
			_squatPrev = squat;
			_inventoryPrev = inventory;
		}

	}


}