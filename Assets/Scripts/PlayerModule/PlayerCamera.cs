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
        [SerializeField] private Material blurMaterial;
        [SerializeField] private Material blackoutMaterial;
        public bool IsInventoryModeOn { get; set; }
        public float MouseSensitivity { get; } = 1;

        public bool IsCutSceneMoving { get; set; } = false;
        
        private static readonly int BlackoutIntensityId = Shader.PropertyToID("_Intensity");
        private float _blackoutIntensity = 1f;
        private float _blackoutIntensityTarget = 0f;
        private float _blackoutIntensitySpeed = .3f;

        private void Start()
        {
            _cameraComponent = GetComponent<Camera>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (IsInventoryModeOn) return;

            UpdateBlackoutIntensity();
            
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
            if (IsCutSceneMoving) return;

            const int size = 8;
            float posX = _cameraComponent.pixelWidth / 2 - size / 2;
            float posY = _cameraComponent.pixelHeight / 2 - size;
            GUI.Label(new Rect(posX, posY, 80, 80), "◦");
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, IsInventoryModeOn ? blurMaterial : blackoutMaterial);
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

        private void UpdateBlackoutIntensity()
        {
            float normalizedSpeed = _blackoutIntensitySpeed * Time.deltaTime;
            
            if (Mathf.Abs(_blackoutIntensity - _blackoutIntensityTarget) <= normalizedSpeed)
            {
                _blackoutIntensity = _blackoutIntensityTarget;
            }
            else
            {
                if (_blackoutIntensity < _blackoutIntensityTarget)
                {
                    _blackoutIntensity += normalizedSpeed;
                }
                else if (_blackoutIntensity > _blackoutIntensityTarget)
                {
                    _blackoutIntensity -= normalizedSpeed;
                }
            }
            
            blackoutMaterial.SetFloat(BlackoutIntensityId, _blackoutIntensity);
        }
        public void Blackout()
        {
            _blackoutIntensityTarget = 1f;
        }
    }
}