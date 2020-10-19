using System;
using Plugins;
using UnityEngine;
using Utils;

namespace PlayerModule
{
    public class PlayerCamera : MonoBehaviour
    {
        private const float BlackoutIntensityMaxChangeStep = 0.01f;
        private const float BlackoutIntensityChangeSpeed = 0.25f;
        private const float FlickerBlackoutIntensityMax = .2f;
        private const float FlickerBlackoutIntensityMin = 0f;
        private const float BlackoutTimeMultiplierMax = 10f;
        private const float BlackoutTimeMultiplierMin = .7f;
        private const float Tolerance = 0.001f;

        private static readonly int BlackoutIntensityId = Shader.PropertyToID("_Intensity");
        private readonly float _mouseVerticalMax = 70;
        private readonly float _mouseVerticalMin = -80;
        private float _blackoutIntensity = 1f;
        private float _blackoutIntensityTarget = 1f;
        private float _blackoutTimeMultiplier = 1f;
        private Camera _cameraComponent;
        private bool _isFlickerAllowed;

        private bool _isFlickerOn;
        private bool _isFlickerSynced;
        private float _rotationX;
        [SerializeField] private Material blackoutMaterial;
        [SerializeField] private Material blurMaterial;
        public bool IsInventoryModeOn { get; set; }
        public float MouseSensitivity { get; } = 1;

        public bool IsCutSceneMoving { get; set; }

        private void Start()
        {
            _cameraComponent = GetComponent<Camera>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            UpdateBlackoutIntensity();

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

        public void FadeOut()
        {
            _blackoutIntensityTarget = 1f;
        }

        public void FadeIn()
        {
            _blackoutIntensityTarget = 0f;
        }

        public void SetFlickerIntensity(float intensity)
        {
            intensity = Mathf.Clamp(intensity, 0f, 1f);
            _blackoutTimeMultiplier = BlackoutTimeMultiplierMin +
                                      (BlackoutTimeMultiplierMax - BlackoutTimeMultiplierMin) * intensity;

            _isFlickerSynced = false;
        }

        public void StopFlicker()
        {
            _isFlickerOn = false;
            _blackoutIntensityTarget = 0f;
        }

        public void StartFlicker()
        {
            _isFlickerOn = true;
        }

        private void UpdateBlackoutIntensity()
        {
            if (!_isFlickerOn)
                _isFlickerAllowed = false;

            bool isIntensityEqualTarget = Math.Abs(_blackoutIntensity - _blackoutIntensityTarget) <= Tolerance;

            if (isIntensityEqualTarget && _isFlickerOn)
            {
                _blackoutIntensity = _blackoutIntensityTarget;
                if (!_isFlickerAllowed)
                    _isFlickerSynced = false;

                _isFlickerAllowed = true;
            }

            float changeSpeed =
                Mathf.Min(BlackoutIntensityChangeSpeed * Time.deltaTime, BlackoutIntensityMaxChangeStep);

            bool isIntensityValueInFlickerRange = _blackoutIntensity > FlickerBlackoutIntensityMin &&
                                                  _blackoutIntensity < FlickerBlackoutIntensityMax;


            bool mustGoToFlickerRange = _isFlickerOn && !_isFlickerAllowed && !isIntensityValueInFlickerRange &&
                                        !isIntensityEqualTarget;
            bool mustGoToTarget = !_isFlickerOn && !isIntensityEqualTarget;

            if (mustGoToFlickerRange)
            {
                _blackoutIntensityTarget = (FlickerBlackoutIntensityMax - FlickerBlackoutIntensityMin) / 2;
                _isFlickerSynced = false;
            }

            if (mustGoToTarget || mustGoToFlickerRange)
            {
                float diff = Math.Abs(_blackoutIntensity - _blackoutIntensityTarget);
                if (diff <= changeSpeed)
                    _blackoutIntensity = _blackoutIntensityTarget;
                else
                    _blackoutIntensity += changeSpeed * (_blackoutIntensity < _blackoutIntensityTarget ? 1 : -1);
            }
            else
            {
                if (_isFlickerOn)
                {
                    float value = (float) ((Math.Sin(Time.time * _blackoutTimeMultiplier) + 1) / 2) *
                                  (FlickerBlackoutIntensityMax - FlickerBlackoutIntensityMin)
                                  + FlickerBlackoutIntensityMin;

                    if (_isFlickerSynced)
                    {
                        _blackoutIntensity = value;
                    }
                    else
                    {
                        float diff = Mathf.Abs(value - _blackoutIntensity);
                        if (diff <= BlackoutIntensityMaxChangeStep)
                        {
                            _isFlickerSynced = true;
                            _blackoutIntensity = value;
                        }
                    }
                }
            }

            blackoutMaterial.SetFloat(BlackoutIntensityId, _blackoutIntensity);

            Debug.Log(_blackoutIntensity + " : " + (_isFlickerOn ? 1 : 0) + " : " +
                      _blackoutTimeMultiplier);

            if (Math.Abs(_blackoutIntensity - 1f) < Tolerance)
                Messenger.Broadcast(Events.FullBlackoutReached);
        }
    }
}