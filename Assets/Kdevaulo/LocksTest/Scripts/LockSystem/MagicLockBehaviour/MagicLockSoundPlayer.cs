using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MagicLockBehaviour
{
    [AddComponentMenu(nameof(MagicLockBehaviour) + "/" + nameof(MagicLockSoundPlayer))]
    public class MagicLockSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _bigWheelAudioSource;
        [SerializeField] private AudioSource _smallWheelAudioSource;
        [SerializeField] private AudioSource _openAudioSource;

        [SerializeField] private AudioClip _openAudioClip;

        public void TryStartPlayingBigWheelSound()
        {
            if (_bigWheelAudioSource.isPlaying)
                return;

            _bigWheelAudioSource.Play();
        }

        public void TryStartPlayingSmallWheelSound()
        {
            if (_smallWheelAudioSource.isPlaying)
                return;

            _smallWheelAudioSource.Play();
        }

        public void TryStopPlayingBigWheelSound()
        {
            if (!_bigWheelAudioSource.isPlaying)
                return;

            _bigWheelAudioSource.Stop();
        }

        public void TryStopPlayingSmallWheelSound()
        {
            if (!_smallWheelAudioSource.isPlaying)
                return;

            _smallWheelAudioSource.Stop();
        }

        public void PlayOpenSound()
        {
            _openAudioSource.PlayOneShot(_openAudioClip);
        }
    }
}