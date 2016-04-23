using Domain;
using System;

namespace MyersDiff
{
    /// <summary>
    /// Согласно Майерсу, змейка - это последовательность длины D-1, оканчивающаяся операцией вставки или удаления.
    /// </summary>
    internal sealed class Snake
    {
        /// <summary>
        /// Количество шагов по диагонали в теущей змейке
        /// </summary>
        public long Diagonals { get; }

        /// <summary>
        /// Элемент, относительно которого начинается змейка
        /// </summary>
        public IComparable Element { get; }

        /// <summary>
        /// Координаты, где оканчивается змейка
        /// </summary>
        public LongPoint End { get; }

        /// <summary>
        /// Тип операции
        /// </summary>
        public OperationKind OperationKind { get; }

        /// <summary>
        /// Координаты, откуда начинается змейка
        /// </summary>
        public LongPoint Start { get; }

        public Snake(OperationKind operationKind, long xStart, long yStart, long xEnd, long yEnd, long diagonals, IComparable element)
        {
            this.Element = element;
            this.OperationKind = operationKind;
            this.End = new LongPoint(xEnd, yEnd);
            this.Start = new LongPoint(xStart, yStart);
            this.Diagonals = diagonals;
        }

        public static Snake CalculateSnake(VArray v, long k, long d, ISource a, long n, ISource b, long m)
        {
            var down = (k == -d || (k != d && v[k - 1] < v[k + 1]));

            var xStart = down ? v[k + 1] : v[k - 1];
            var yStart = xStart - (down ? k + 1 : k - 1);
            var xEnd = down ? xStart : xStart + 1;
            var yEnd = xEnd - k;

            var snake = 0;
            while (xEnd < n && yEnd < m && Equals(a[xEnd], b[yEnd])) { xEnd++; yEnd++; snake++; }
            var snk = new Snake(yStart < 0 ? OperationKind.Equal : (down ? OperationKind.Insert : OperationKind.Delete),
                                xStart, yStart, xEnd, yEnd, snake, down ? (yStart < 0 ? ' ' : b[yStart]) : a[xStart]);
            return snk;
        }

        public override string ToString()
        {
            return $"{this.OperationKind} Start: {this.Start} End: {this.End} {(this.OperationKind == OperationKind.Equal ? string.Empty : $"Element: {this.Element}")} Diagonals: {this.Diagonals}";
        }
    }
}