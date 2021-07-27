namespace SidokuSolver_WinForm
{
    public class Step
    {
        public int X;
        public int Y;
        public int Value;

        public Step(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }
}
