using UnityEngine;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    public class TransformRotator
    {
        private readonly Transform _targetTransform;

        private readonly Camera _currentCamera;

        private Quaternion _originalRotation;

        private float _startAngle;

        private Vector2 _screenPosition;
        private Vector2 _currentOffset;

        private bool _canRotate = true;

        public TransformRotator(Transform targetTransform, Camera targetCamera)
        {
            _targetTransform = targetTransform;
            _currentCamera = targetCamera;
        }

        public void DisableRotation()
        {
            _canRotate = false;
        }

        public void HandlePointerDown()
        {
            _originalRotation = _targetTransform.rotation;

            CalculateOffset();

            _startAngle = Mathf.Atan2(_currentOffset.y, _currentOffset.x) * Mathf.Rad2Deg;
        }

        public void HandlePointerDrag()
        {
            if (!_canRotate)
                return;

            CalculateOffset();

            Quaternion targetRotation = GetRotation(_currentOffset);

            targetRotation.y = 0; // note: may need to be changed depending on what axis the object is rotating on
            targetRotation.eulerAngles = new Vector3(0, 0, targetRotation.eulerAngles.z);

            _targetTransform.rotation = _originalRotation * targetRotation;
        }

        private void CalculateOffset()
        {
            SetScreenPosition();

#if UNITY_IPHONE || UNITY_ANDROID
            SetOffsetByTouch();
#else
            SetOffsetByMouse();
#endif
        }

        private void SetScreenPosition()
        {
            _screenPosition = _currentCamera.WorldToScreenPoint(_targetTransform.position);
        }

        private void SetOffsetByTouch()
        {
            SetCurrentOffset(Input.GetTouch(0).position);
        }

        private void SetOffsetByMouse()
        {
            SetCurrentOffset(Input.mousePosition);
        }

        private void SetCurrentOffset(Vector2 pointerPosition)
        {
            _currentOffset = pointerPosition - _screenPosition;
        }

        private Quaternion GetRotation(Vector2 vector)
        {
            var angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            return Quaternion.AngleAxis(angle - _startAngle, _targetTransform.forward);
        }
    }
}