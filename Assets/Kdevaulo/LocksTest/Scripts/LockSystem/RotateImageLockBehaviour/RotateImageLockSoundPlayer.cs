using System;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    [AddComponentMenu(nameof(RotateImageLockBehaviour) + "/" + nameof(RotateImageLockSoundPlayer))]
    public class RotateImageLockSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _rotateAudioSource;
        [SerializeField] private AudioSource _onStartRotationAudioSource;
        [SerializeField] private AudioSource _openAudioSource;

        [SerializeField] private AudioClip _rotateAudioClip;
        [SerializeField] private AudioClip _openAudioClip;
        [SerializeField] private AudioClip _onStartRotationAudioClip;

        private bool _isRotationPlaying;

        public void PlayOpenSound()
        {
            _openAudioSource.PlayOneShot(_openAudioClip);
        }

        public void PlayStartRotationSound()
        {
            _onStartRotationAudioSource.PlayOneShot(_onStartRotationAudioClip);
        }

        public async UniTask PlayRotateSoundAsync()
        {
            if (_isRotationPlaying)
                return;

            _isRotationPlaying = true;

            PlayRotateSound();

            await UniTask.Delay(TimeSpan.FromSeconds(_rotateAudioClip.length));

            _isRotationPlaying = false;
        }

        private void PlayRotateSound()
        {
            _rotateAudioSource.PlayOneShot(_rotateAudioClip);
        }
    }
}