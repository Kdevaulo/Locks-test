using System;

using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.ClickLockBehaviour
{
    [Serializable]
    public class PinKit
    {
        public bool Activated => _activated;

        public SpriteRenderer Led;

        [SerializeField] private Transform _standartPoint;

        [SerializeField] private Transform _correctPoint;

        [SerializeField] private Transform _pin;

        private bool _activated;

        public void SetLedColor(Color color)
        {
            Led.color = color;
        }

        public void SetActive(bool state)
        {
            _activated = state;
        }

        public void SetStandartPinPosition()
        {
            _pin.localPosition = _standartPoint.localPosition;
        }

        public void SetCorrectPinPosition()
        {
            _pin.localPosition = _correctPoint.localPosition;
        }
    }
}