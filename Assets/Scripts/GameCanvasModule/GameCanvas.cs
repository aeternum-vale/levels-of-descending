using System;
using Plugins;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GameCanvasModule
{
    public class GameCanvas : MonoBehaviour
    {
        private const float BlackoutIntensityMaxChangeStep = 0.01f;
        private const float BlackoutIntensityChangeSpeed = 0.25f;
        private const float FlickerBlackoutIntensityMax = .2f;
        private const float FlickerBlackoutIntensityMin = 0f;
        private const float BlackoutTimeMultiplierMax = 10f;
        private const float BlackoutTimeMultiplierMin = .7f;
        private const float Tolerance = 0.001f;

        private Image _backImage;

        private float _blackoutIntensity = 1f;
        private float _blackoutIntensityTarget = 1f;
        private float _blackoutTimeMultiplier = 1f;
        private bool _isFlickerAllowed;

        private bool _isFlickerOn;
        private bool _isFlickerSynced;

		private CanvasGroup _backImageCanvasGroup;

        public void FadeOut()
        {
            _isFlickerOn = false;
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
            float changeSpeed =
                Mathf.Min(BlackoutIntensityChangeSpeed * Time.deltaTime, BlackoutIntensityMaxChangeStep);

            if (!_isFlickerOn)
                _isFlickerAllowed = false;

            if (_isFlickerOn && _isFlickerAllowed)
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
                    if (diff <= changeSpeed)
                    {
                        _isFlickerSynced = true;
                        _blackoutIntensity = value;
                    }
                }
            }
            else
            {
                if (_isFlickerOn)
                {
                    _blackoutIntensityTarget = FlickerBlackoutIntensityMin +
                                               (FlickerBlackoutIntensityMax - FlickerBlackoutIntensityMin) / 2;
                    _isFlickerSynced = false;
                }

                float diff = Math.Abs(_blackoutIntensity - _blackoutIntensityTarget);
                if (diff <= changeSpeed)
                    _blackoutIntensity = _blackoutIntensityTarget;
                else
                    _blackoutIntensity += changeSpeed * (_blackoutIntensity < _blackoutIntensityTarget ? 1 : -1);

                bool isIntensityEqualTarget = Math.Abs(_blackoutIntensity - _blackoutIntensityTarget) <= Tolerance;
                if (isIntensityEqualTarget && _isFlickerOn)
                {
                    _blackoutIntensity = _blackoutIntensityTarget;

                    if (_isFlickerOn)
                    {
                        _isFlickerAllowed = true;
                        _isFlickerSynced = false;
                    }
                }
            }

			SetBackImageAlpha(_blackoutIntensity);

            /*Debug.Log(_blackoutIntensity + " : " + (_isFlickerOn ? 1 : 0) + " : " +
                      _blackoutTimeMultiplier);*/

            if (Math.Abs(_blackoutIntensity - 1f) < Tolerance)
                Messenger.Broadcast(Events.FullBlackoutReached);
        }

        private void Awake()
        {
            _backImage = transform.Find("backImage").GetComponent<Image>();
			_backImageCanvasGroup = _backImage.GetComponent<CanvasGroup>();
        }

		private void SetBackImageAlpha(float alpha)
		{
			_backImageCanvasGroup.alpha = alpha;
		}

        private void Start()
        {
            _isFlickerOn = true;
        }

        private void Update()
        {
            UpdateBlackoutIntensity();
        }
    }
}