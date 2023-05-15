using System;

using UnityEngine;
using UnityEngine.UI;

namespace Kdevaulo.LocksTest.Scripts.LockSystem
{
    public class PressToOpenScreenController : IDisposable
    {
        public event Action StartPressed = delegate { };

        private readonly Canvas _screenCanvas;
        private readonly Button _startButton;

        public PressToOpenScreenController(Canvas screenCanvas, Button startButton)
        {
            _screenCanvas = screenCanvas;
            _startButton = startButton;

            startButton.onClick.AddListener(HandleStartButtonClick);
        }

        void IDisposable.Dispose()
        {
            _startButton.onClick.RemoveListener(HandleStartButtonClick);
        }

        public void EnableScreen()
        {
            _screenCanvas.gameObject.SetActive(true);
        }

        private void HandleStartButtonClick()
        {
            _screenCanvas.gameObject.SetActive(false);

            StartPressed.Invoke();
        }
    }
}