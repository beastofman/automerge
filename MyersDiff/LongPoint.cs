namespace MyersDiff
{
    internal sealed class intPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public intPoint()
        {
            this.X = this.Y = 0;
        }

        public intPoint(int x, int y)
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