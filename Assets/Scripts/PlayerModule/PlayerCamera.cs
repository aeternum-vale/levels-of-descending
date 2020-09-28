using System;
using UnityEngine;
using Utils;

namespace PlayerModule
{
    public class PlayerCamera : MonoBehaviour
    {
        private readonly float _mouseVerticalMax = 70;
        private readonly float _mouseVerticalMin = -80;
        private Camera _cameraComponent;
        private float _rotationX;
        [SerializeField] private Material mat;
        public bool IsInventoryModeOn { get; set; }
        public float MouseSensitivity { get; } = 1;

        public bool IsCutSceneMoving { get; set; } = false;

        private void Start()
        {
            _cameraComponent = GetComponent<Camera>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (IsInventoryModeOn) return;

            if (IsCutSceneMoving)
            {
                _rotationX = NormalizeAngle(transform.localEulerAngles.x, -90, 90);
                return;
            }

            _rotationX -= Input.GetAxis("Mouse Y") * MouseSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, _mouseVerticalMin, _mouseVerticalMax);
            transform.localEulerAngles = new Vector3(_rotationX, 0, 0);
        }

        private static int NormalizeAngle(float value, int start, int end)
        {
            int width = end - start;
            float offsetValue = value - start;

            return (int) (offsetValue + start - (int) (offsetValue / width) * width);
        }

        private void OnGUI()
        {
            if (IsInventoryModeOn) return;

            const int size = 8;
            float posX = _cameraComponent.pixelWidth / 2 - size / 2;
            float posY = _cameraComponent.pixelHeight / 2 - size;
            GUI.Label(new Rect(posX, posY, 80, 80), "◦");
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (IsInventoryModeOn)
                Graphics.Blit(src, dest, mat);
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