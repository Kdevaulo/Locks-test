using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour
{
    [AddComponentMenu(nameof(ClickLockBehaviour) + "/" + nameof(ClickSoundPlayer))]
    public class ClickSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [Tooltip("Clips count should be == pins count")]
        [SerializeField] private AudioClip[] _audioClips;

        /// <param name="activationNumber">Activation order number (startPoint == 0)</param>
        public void PlaySound(int activationNumber)
        {
            _audioSource.PlayOneShot(_audioClips[activationNumber]);
        }
    }
}