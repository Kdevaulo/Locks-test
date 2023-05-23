using UnityEngine;
using UnityEngine.UI;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    [AddComponentMenu(nameof(LockSystem) + "/" + nameof(ScoreView))]
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private Text _scoreValue;

        public void SetScore(int value)
        {
            _scoreValue.text = value.ToString();
        }
    }
}