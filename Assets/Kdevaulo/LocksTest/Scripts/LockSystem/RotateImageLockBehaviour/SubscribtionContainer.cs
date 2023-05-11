using System;

namespace Kdevaulo.LocksTest.Scripts.LockSystem.RotateImageLockBehaviour
{
    public class SubscriptionContainer
    {
        public Action PointerDragAction;
        public Action PointerDownAction;

        public UserInputHandler InputHandler;

        public SubscriptionContainer(Action pointerDragAction, Action pointerDownAction,
            UserInputHandler userInputHandler)
        {
            PointerDragAction = pointerDragAction;
            PointerDownAction = pointerDownAction;

            InputHandler = userInputHandler;
        }
    }
}