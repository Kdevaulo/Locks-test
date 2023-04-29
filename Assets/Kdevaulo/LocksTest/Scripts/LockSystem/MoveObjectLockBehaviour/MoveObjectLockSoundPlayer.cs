using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    [AddComponentMenu(nameof(MoveObjectLockBehaviour) + "/" + nameof(MoveObjectLockSoundPlayer))]
    public class MoveObjectLockSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _breakAudioClip;

        [SerializeField] private AudioClip _openAudioClip;

        public void PlayBreakSound()
        {
            _audioSource.PlayOneShot(_breakAudioClip);
        }

        public void PlayOpenSound()
        {
            _audioSource.PlayOneShot(_openAudioClip);
        }
    }
}