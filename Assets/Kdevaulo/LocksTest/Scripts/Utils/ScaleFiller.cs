using System;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.Utils
{
    public class ScaleFiller
    {
        public event Action MaxReached = delegate { };
        public event Action MinReached = delegate { };

        private readonly Vector2 _startPoint;

        private readonly Vector2 _endPoint;

        private readonly float _fillStep;

        private Vector2 _currentPoint;

        private float _currentFillValue = 0;

        public ScaleFiller(Vector2 startPoint, Vector2 endPoint, float fillStep)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
            _fillStep = fillStep;
        }

        public Vector2 GetIncreasePoint()
        {
            _currentFillValue += _fillStep;

            LerpCurrentPointValue();

            if (_currentFillValue >= 1)
            {
                MaxReached.Invoke();
            }

            return _currentPoint;
        }

        public Vector2 GetDecreasePoint()
        {
            _currentFillValue -= _fillStep;

            LerpCurrentPointValue();

            if (_currentFillValue <= 0)
            {
                MinReached.Invoke();
            }

            return _currentPoint;
        }

        private void LerpCurrentPointValue()
        {
            _currentPoint = Vector2.Lerp(_startPoint, _endPoint, _currentFillValue);
        }
    }
}