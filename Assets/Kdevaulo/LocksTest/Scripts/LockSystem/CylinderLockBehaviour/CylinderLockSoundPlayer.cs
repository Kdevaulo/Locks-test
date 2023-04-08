using System;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.CylinderLockBehaviour
{
    [AddComponentMenu(nameof(CylinderLockBehaviour) + "/" + nameof(CylinderLockSoundPlayer))]
    public class CylinderLockSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _rotateAudioSource;

        [SerializeField] private AudioSource _openAudioSource;

        [SerializeField] private AudioClip _rotateAudioClip;

        [SerializeField] private AudioClip _openAudioClip;

        private bool _isRotationPlaying;

        private bool _isOpenPlaying;

        public void PlayRotateSound()
        {
            if (_isRotationPlaying)
            {
                return;
            }

            PlayRotateSoundAsync().Forget();
        }

        public void PlayOpenSound()
        {
            if (_isOpenPlaying)
            {
                return;
            }

            _isOpenPlaying = true;
            _openAudioSource.PlayOneShot(_openAudioClip);
        }

        private async UniTask PlayRotateSoundAsync()
        {
            _isRotationPlaying = true;

            _rotateAudioSource.PlayOneShot(_rotateAudioClip);

            await UniTask.Delay(TimeSpan.FromSeconds(_rotateAudioClip.length));

            _isRotationPlaying = false;
        }
    }
}