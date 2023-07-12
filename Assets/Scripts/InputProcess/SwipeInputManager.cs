using System;
using UnityEngine;

namespace InputProcess
{
    public class SwipeInputManager : IInputManager
    {
        private Vector3 _startPosition;
        private const int MinSwipeDistance = 100;

        private Action<InputResult> _onSwipeInput;

        public InputResult GetInput()
        {
            var result = new InputResult();

            switch (GetSwipeState())
            {
                case SwipeState.Start:
                    _startPosition = Input.mousePosition;
                    break;
                case SwipeState.End:
                    var delta = Input.mousePosition - _startPosition;
                    if (delta.magnitude >= MinSwipeDistance)
                    {
                        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                            result.XInp = Math.Sign(delta.x);
                        else
                            result.YInp = Math.Sign(delta.y);

                        _onSwipeInput?.Invoke(result);
                    }
                    break;
                case SwipeState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public void Subscribe(Action<InputResult> action)
        {
            _onSwipeInput += action;
        }

        public void Unsubscribe(Action<InputResult> action)
        {
            _onSwipeInput -= action;
        }

        private enum SwipeState
        {
            None,
            Start,
            End
        }

        private SwipeState GetSwipeState()
        {
            if (Input.GetMouseButtonDown(0))
                return SwipeState.Start;
            
            return Input.GetMouseButtonUp(0) ? SwipeState.End : SwipeState.None;
        }
    }
}