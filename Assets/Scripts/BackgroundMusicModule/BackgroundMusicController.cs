using System;
using System.Collections;
using UnityEngine;

namespace BackgroundMusicModule
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicController : MonoBehaviour
    {
        private const float VolumeChangeSpeed = 0.01f;

        private AudioSource _audioSource;

        private float _backgroundMusicIntensity;
        private bool _clipChangingIsOn;
        private float _currentVolume;

        [SerializeField] private AudioClip[] clips;
        [Range(0f, 1f)] [SerializeField] private float maxVolume = 1f;

        public float BackgroundMusicIntensity
        {
            get => _backgroundMusicIntensity;
            set
            {
                _backgroundMusicIntensity = Mathf.Clamp(value, 0f, 1f);
                AudioClip nextClip = GetAppropriateClip();
                _currentVolume = GetAppropriateVolume();

                if (_audioSource.clip == null)
                {
                    _audioSource.clip = nextClip;
                    _audioSource.Play();
                }
                else
                {
                    if (!_clipChangingIsOn)
                        StartCoroutine(ChangeClipAndCurrentVolume(nextClip));
                }

                Debug.Log("backgroundMusicIntensity: " + _backgroundMusicIntensity);
                Debug.Log("currentVolume: " + _currentVolume);
            }
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _currentVolume = GetAppropriateVolume();
            _audioSource.volume = _currentVolume;
        }

        private AudioClip GetAppropriateClip()
        {
            int index = (int) (clips.Length * _backgroundMusicIntensity);
            if (index == clips.Length) index--;

            return clips[index];
        }

        private float GetAppropriateVolume()
        {
            return Mathf.Clamp(_backgroundMusicIntensity * maxVolume, 0.01f, 1f);
        }

        private IEnumerator ChangeClipAndCurrentVolume(AudioClip nextClip)
        {
            _clipChangingIsOn = true;

            if (_audioSource.clip != nextClip)
            {
                yield return StartCoroutine(ChangeVolumeTo(0f));
                _audioSource.clip = nextClip;
                _audioSource.Play();
            }

            yield return StartCoroutine(ChangeVolumeTo(_currentVolume));
            _clipChangingIsOn = false;
        }

        private IEnumerator ChangeVolumeTo(float value)
        {
            while (Math.Abs(_audioSource.volume - value) > VolumeChangeSpeed)
            {
                float volume = _audioSource.volume;
                volume += VolumeChangeSpeed * (volume - value > 0 ? -1 : 1);
                volume = Mathf.Clamp(volume, 0f, 1f);
                _audioSource.volume = volume;

                yield return new WaitForFixedUpdate();
            }

            _audioSource.volume = value;
        }
    }
}