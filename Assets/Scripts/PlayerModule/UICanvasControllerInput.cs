using UnityEngine;

namespace PlayerModule
{
	public class UICanvasControllerInput : MonoBehaviour
	{

		[Header("Output")]
		public InputsCombiner _inputsCombiner;

		public void VirtualMoveInput(Vector2 virtualMoveDirection)
		{
			_inputsCombiner.MoveInput(virtualMoveDirection);
		}

		public void VirtualLookInput(Vector2 virtualLookDirection)
		{
			_inputsCombiner.LookInput(virtualLookDirection);
		}

		public void VirtualUseInput(bool virtualUseState)
		{
			_inputsCombiner.UseInput(virtualUseState);
		}

		public void VirtualSquatInput(bool virtualSquatState)
		{
			_inputsCombiner.SquatInput(virtualSquatState);
		}

		public void VirtualInventoryInput(bool virtualInventoryState)
		{
			_inputsCombiner.InventoryInput(virtualInventoryState);
		}



	}

}
