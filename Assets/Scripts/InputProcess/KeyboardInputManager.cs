using UnityEngine;

namespace InputProcess
{
    public class KeyboardInputManager : IInputManager
    {
        private int _lastXInp;
        private int _lastYInp;

        public InputResult GetInput()
        {
            var result = new InputResult();

            var xInp = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            var yInp = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

            if (_lastXInp == 0 && _lastYInp == 0)
            {
                result.XInp = xInp;
                result.YInp = yInp;
            }

            _lastXInp = xInp;
            _lastYInp = yInp;

            return result;
        }
    }
}