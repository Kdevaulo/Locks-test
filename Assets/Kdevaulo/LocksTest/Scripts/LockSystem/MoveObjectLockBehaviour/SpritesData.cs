using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.MoveObjectLockBehaviour
{
    [CreateAssetMenu(menuName = nameof(MoveObjectLockBehaviour) + "/" + nameof(SpritesData),
        fileName = nameof(SpritesData))]
    public class SpritesData : ScriptableObject
    {
        [SerializeField] private Sprite[] _movingItems;

        public Sprite GetRandomItem()
        {
            return _movingItems[Random.Range(0, _movingItems.Length)];
        }
    }
}