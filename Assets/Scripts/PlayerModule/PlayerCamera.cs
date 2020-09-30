using System;
using System.Collections;
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
        private static readonly float BlackoutIntensitySpeed = .005f;
        private static readonly float BlackoutIdleIntensityMax = .25f;
        private static readonly float BlackoutIdleIntensityMin = 0f;
        private static readonly float BlackoutIdleTimeMultiplier = 1f;
        private bool _blackoutIntensityIsLocked = false;

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


        public IEnumerator FadeOut()
        {
            _blackoutIntensityIsLocked = true;
            yield return StartCoroutine(AnimateBlackoutIntensity(1f));
        }

        public IEnumerator FadeIn()
        {
            _blackoutIntensityIsLocked = true;
            yield return StartCoroutine(AnimateBlackoutIntensity(0f));
            _blackoutIntensityIsLocked = false;
        }

        private IEnumerator AnimateBlackoutIntensity(float target)
        {
            while (Mathf.Abs(_blackoutIntensity - target) > BlackoutIntensitySpeed)
            {
                if (_blackoutIntensity < target)
                    _blackoutIntensity += BlackoutIntensitySpeed;
                else if (_blackoutIntensity > target)
                    _blackoutIntensity -= BlackoutIntensitySpeed;

                yield return new WaitForFixedUpdate();
            }

            _blackoutIntensity = target;
            yield return null;
        }

        private void UpdateBlackoutIntensity()
        {
            Debug.Log(_blackoutIntensity);
            if (!_blackoutIntensityIsLocked)
            {
                float value = (float)((Math.Sin(Time.time  * BlackoutIdleTimeMultiplier) + 1) / 2) *
                    (BlackoutIdleIntensityMax - BlackoutIdleIntensityMin) + BlackoutIdleIntensityMin;

                float diff = Mathf.Abs(value - _blackoutIntensity);
                if (diff <= BlackoutIntensitySpeed)
                    _blackoutIntensity = value;
            }

            blackoutMaterial.SetFloat(BlackoutIntensityId, _blackoutIntensity);
        }
    }
}