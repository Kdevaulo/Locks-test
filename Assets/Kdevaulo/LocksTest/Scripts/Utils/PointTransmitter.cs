using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.Utils
{
    public class PointTransmitter
    {
        private readonly Camera _camera;

        public PointTransmitter(Camera camera)
        {
            _camera = camera;
        }

        public Vector2 TransmitPoint(Vector2 position)
        {
            var point = _camera.ScreenToWorldPoint(position);

            point.z = 0;

            return point;
        }
    }
}