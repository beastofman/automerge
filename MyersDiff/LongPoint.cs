namespace MyersDiff
{
    internal sealed class LongPoint
    {
        public long X { get; set; }
        public long Y { get; set; }

        public LongPoint()
        {
            this.X = this.Y = 0;
        }

        public LongPoint(long x, long y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"({this.X};{this.Y})";
        }
    }
}