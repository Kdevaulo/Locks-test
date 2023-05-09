using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ResistanceClickLockBehaviour
{
    [AddComponentMenu(nameof(ResistanceClickLockBehaviour) + "/" + nameof(ResistanceClickLockSoundPlayer))]
    public class ResistanceClickLockSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _oneShotSource;
        [SerializeField] private AudioSource _soundSource;

        [SerializeField] private AudioClip _winSound;
        [SerializeField] private AudioClip _loseSound;

        public void PlayLoseSound()
        {
            _oneShotSource.PlayOneShot(_loseSound);
        }

        public void PlayWinSound()
        {
            _oneShotSource.PlayOneShot(_winSound);
        }

        public void StartPlayBackgroundBattleSound()
        {
            _soundSource.Play();
        }

        public void StopBattleSound()
        {
            _soundSource.Stop();
        }
    }
}