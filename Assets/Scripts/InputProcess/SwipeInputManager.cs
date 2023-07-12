using System;
using UnityEngine;

namespace InputProcess
{
    public class SwipeInputManager : IInputManager
    {
        private bool _isSwipeMove;
        private Vector3 _startPosition;

        private const int MinSwipeDistance = 100;

        public InputResult GetInput()
        {
            var result = new InputResult();

            if (!_isSwipeMove)
            {
                if (!Input.GetMouseButton(0)) return result;
                _isSwipeMove = true;
                _startPosition = Input.mousePosition;
            }
            else
            {
                if (Input.GetMouseButton(0)) return result;
                _isSwipeMove = false;
                var delta = Input.mousePosition - _startPosition;
                if (!(delta.magnitude >= MinSwipeDistance)) return result;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    result.XInp = Math.Sign(delta.x);
                }
                else
                {
                    result.YInp = Math.Sign(delta.y);
                }
            }
            return result;
        }
    }
}