using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace BackgroundMusicModule
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicController : MonoBehaviour
    {
        private const float VolumeChangeRate = 0.008f;

        private AudioSource _audioSource;

        private float _backgroundMusicIntensity;
        private bool _intensityChangingIsOn;

        [SerializeField] private AudioClip[] clips;
        [Range(0f, 1f)] [SerializeField] private float maxVolume = 1f;

        public float BackgroundMusicIntensity
        {
            set
            {
                _backgroundMusicIntensity = Mathf.Clamp(value, 0f, 1f);

                if (!_intensityChangingIsOn)
                    StartCoroutine(ChangeClipAndIntensityVolumeToAppropriate());
            }
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.volume = 0f;
        }

        private AudioClip GetAppropriateClip()
        {
            int index = (int) (clips.Length * _backgroundMusicIntensity);
            if (index == clips.Length) index--;
            return _backgroundMusicIntensity <= 0f ? null : clips[index];
        }

        private float GetAppropriateVolume()
        {
            return _backgroundMusicIntensity * maxVolume;
        }

        private IEnumerator ChangeClipAndIntensityVolumeToAppropriate()
        {
            _intensityChangingIsOn = true;

            AudioClip nextClip = GetAppropriateClip();

            if (_audioSource.clip != nextClip)
            {
                yield return StartCoroutine(ChangeVolumeTo(0f));
                _audioSource.clip = nextClip;
                _audioSource.Play();
            }

            yield return StartCoroutine(ChangeVolumeTo(GetAppropriateVolume()));
            _intensityChangingIsOn = false;
        }

        private IEnumerator ChangeVolumeTo(float target)
        {
            return GameUtils.AnimateValue(
                () => _audioSource.volume,
                value => _audioSource.volume = value,
                target,
                VolumeChangeRate);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}