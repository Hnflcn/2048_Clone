namespace InputProcess
{
    public class InputResult
    {
        public int XInp = 0;
        public int YInp = 0;

        public bool HasValue => XInp != 0 || YInp != 0;
    }
}