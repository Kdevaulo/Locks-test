using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    [AddComponentMenu(nameof(MoveObjectLockBehaviour) + "/" + nameof(MoveObjectLockSoundPlayer))]
    public class MoveObjectLockSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _oneShotAudioSource;
        [SerializeField] private AudioSource _soundAudioSource;

        [SerializeField] private AudioClip _breakAudioClip;
        [SerializeField] private AudioClip _openAudioClip;

        public void PlayBreakSound()
        {
            _oneShotAudioSource.PlayOneShot(_breakAudioClip);
        }

        public void PlayOpenSound()
        {
            _oneShotAudioSource.PlayOneShot(_openAudioClip);
        }

        public void StartPlayLockSound()
        {
            _soundAudioSource.Play();
        }

        public void StopLockSound()
        {
            _soundAudioSource.Stop();
        }
    }
}