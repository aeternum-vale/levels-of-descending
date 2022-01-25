using UnityEngine;
using Utils;

namespace PlayerModule
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Material blurMaterial;
		
        private Camera _cameraComponent;
        private float _rotationX;

        public bool IsInventoryModeOn { get; set; }
        public bool IsCutSceneMoving { get; set; }
		public float RotationX { get => _rotationX; set => _rotationX = value; }

		private void Start()
        {
            _cameraComponent = GetComponent<Camera>();
            //Cursor.lockState = CursorLockMode.Locked;
           // Cursor.visible = false;
        }

        // private void Update()
        // {
        //     if (IsInventoryModeOn) return;

        //     if (IsCutSceneMoving)
        //     {
		// 		transform.localEulerAngles = new Vector3(NormalizeAngle(transform.localEulerAngles.x, -90, 90), 0, 0);
        //         return;
        //     }
        // }


        private void OnGUI()
        {
            if (IsInventoryModeOn) return;
            if (IsCutSceneMoving) return;

            const int size = 8;
            float posX = _cameraComponent.pixelWidth / 2 - size / 2;
            float posY = _cameraComponent.pixelHeight / 2 - size;
            GUI.Label(new Rect(posX, posY, 80, 80), "◦");
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (IsInventoryModeOn)
                Graphics.Blit(src, dest, blurMaterial);
            else
                Graphics.Blit(src, dest);
        }

        public Texture2D GetBackgroundTexture()
        {
            return CameraUtils.GetCameraTexture(_cameraComponent, Screen.width, Screen.height);
        }

        public void ActivateInventoryMode()
        {
            IsInventoryModeOn = true;
            gameObject.SetActive(false);
        }

        public void DeactivateInventoryMode()
        {
            IsInventoryModeOn = false;
            _cameraComponent.targetTexture = null;
            gameObject.SetActive(true);
        }
    }
}