using UnityEngine;

namespace InputProcess
{
    public class KeyboardInputManager : IInputManager
    {
        private readonly InputResult _lastInputResult=  new InputResult();

        public InputResult GetInput()
        {
            var result = new InputResult();

            var xInp = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            var yInp = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

            if (!_lastInputResult.HasValue)
            {
                result.XInp = xInp;
                result.YInp = yInp;
            }

            _lastInputResult.XInp = xInp;
            _lastInputResult.YInp = yInp;

            return result;
        }
    }
}