using Domain;
using System;

namespace MyersDiff
{
    /// <summary>
    /// Согласно Майерсу, змейка - это путь длиной D-1, начинающийся с операции вставки или удаления, и оканчивающийся последовательностью диагональных шагов (возможно пустой).
    /// </summary>
    internal sealed class Snake
    {
        /// <summary>
        /// D - число операций удаления или вставки для этой змейки
        /// </summary>
        public int D { get; }

        /// <summary>
        /// Количество шагов по диагонали в текущей змейке
        /// </summary>
        public int Diagonals { get; }

        /// <summary>
        /// Элемент, относительно которого начинается змейка
        /// </summary>
        public IComparable Element { get; }

        /// <summary>
        /// Координаты, где оканчивается змейка
        /// </summary>
        public intPoint End { get; }

        /// <summary>
        /// Тип операции
        /// </summary>
        public OperationKind OperationKind { get; }

        /// <summary>
        /// Координаты, откуда начинается змейка
        /// </summary>
        public intPoint Start { get; }

        public Snake(OperationKind operationKind, int xStart, int yStart, int xEnd, int yEnd, int d, int diagonals, IComparable element)
        {
            this.Element = element;
            this.OperationKind = operationKind;
            this.End = new intPoint(xEnd, yEnd);
            this.Start = new intPoint(xStart, yStart);
            this.Diagonals = diagonals;
            this.D = d;
        }

        /// <summary>
        /// Вычисление змейки
        /// </summary>
        /// <param name="v">Массив V</param>
        /// <param name="k">Диагональ, относительно которой ищем максимально протяженный D-путь</param>
        /// <param name="d">Длина D-пути</param>
        /// <param name="a">Оригинал</param>
        /// <param name="n">Длина оригинала</param>
        /// <param name="b">Источник с изменениями</param>
        /// <param name="m">Длина источника с изменениями</param>
        /// <returns></returns>
        public static Snake CalculateSnake(VArray v, int k, int d, ISource a, int n, ISource b, int m)
        {
            var down = (k == -d || (k != d && v[k - 1] < v[k + 1]));

            var xStart = down ? v[k + 1] : v[k - 1];
            var yStart = xStart - (down ? k + 1 : k - 1);
            var xEnd = down ? xStart : xStart + 1;
            var yEnd = xEnd - k;

            var snake = 0;
            while (xEnd < n && yEnd < m && Equals(a[xEnd], b[yEnd])) { xEnd++; yEnd++; snake++; }
            var snk = new Snake(yStart < 0 ? OperationKind.Equal : (down ? OperationKind.Insert : OperationKind.Delete),
                                xStart, yStart, xEnd, yEnd, d, snake, down ? (yStart < 0 ? ' ' : b[yStart]) : a[xStart]);
            return snk;
        }

        public override string ToString()
        {
            return $"{this.OperationKind} Start: {this.Start} End: {this.End} {(this.OperationKind == OperationKind.Equal ? string.Empty : $"Element: {this.Element}")} D:{this.D} Diagonals: {this.Diagonals}";
        }
    }
}