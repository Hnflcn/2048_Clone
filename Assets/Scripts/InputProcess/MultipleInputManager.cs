using System.Linq;

namespace InputProcess
{
    public class MultipleInputManager : IInputManager
    {
        private readonly IInputManager[] _managers;

        public MultipleInputManager(params IInputManager[] managers)
        {
            _managers = managers;
        }

        public InputResult GetInput()
        {
            return _managers.Select(manager => manager.GetInput())
                       .FirstOrDefault(inp => inp.HasValue)
                   ?? new InputResult();
        }
    }
}