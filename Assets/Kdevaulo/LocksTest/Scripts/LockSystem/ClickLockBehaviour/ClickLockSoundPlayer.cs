using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour
{
    [AddComponentMenu(nameof(ClickLockBehaviour) + "/" + nameof(ClickLockSoundPlayer))]
    public class ClickLockSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _clickAudioSource;

        [SerializeField] private AudioSource _tickAudioSource;

        [Tooltip("Clips count should be == pins count")]
        [SerializeField] private AudioClip[] _clickAudioClips;

        [SerializeField] private AudioClip _tickAudioClip;

        /// <param name="activationNumber">Activation order number (startPoint == 0)</param>
        public void PlayClickSound(int activationNumber)
        {
            _clickAudioSource.PlayOneShot(_clickAudioClips[activationNumber]);
        }

        public void PlayMoveLedSound()
        {
            _tickAudioSource.PlayOneShot(_tickAudioClip);
        }
    }
}