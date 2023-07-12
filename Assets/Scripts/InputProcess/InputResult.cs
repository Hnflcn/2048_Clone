namespace InputProcess
{
    public class InputResult
    {
        public int XInp { get; set; }
        public int YInp { get; set; }

        public bool HasValue => XInp != 0 || YInp != 0;
    }
}