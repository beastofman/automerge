using Domain;
using System.Collections.Generic;

namespace MyersDiff
{
    /// <summary>
    /// Движок для поиска различий между двумя источниками, основанный на алгоритме Diff2 Юджина Майерса.
    /// Используется неоптимизированная версия алгоритма.
    /// Подробнее здесь: An O(ND) Difference Algorithm and Its Variations by EUGENE W. MYERS  http://xmailserver.org/diff2.pdf
    /// </summary>
    public class MyersDiffEngine : IDiffEngine
    {
        public IEnumerable<Operation> GetDiff(ISource original, ISource target)
        {
            var a = original;
            var b = target;

            var n = a.Length;
            var m = b.Length;

            if (n == 0 ||
                m == 0) return null;

            var vs = new List<VArray>();
            var v = new VArray(n, m) { [1] = 0 };
            var hasSolution = false;

            for (var d = 0; d < n + m; d++)
            {
                for (var k = -d; k <= d; k += 2)
                {
                    var down = (k == -d || (k != d && v[k - 1] < v[k + 1]));

                    var xStart = down ? v[k + 1] : v[k - 1];

                    var xEnd = xStart + (down ? 0 : 1);
                    var yEnd = xEnd - k;

                    while (xEnd < n &&
                           yEnd < m &&
                           Equals(a[xEnd], b[yEnd]))
                    {
                        xEnd++;
                        yEnd++;
                    }
                    v[k] = xEnd;

                    if (xEnd < n ||
                        yEnd < m) continue;

                    hasSolution = true;
                    break;
                }
                vs.Add(v.Copy(d));
                if (hasSolution) break;
            }

            if (!hasSolution)
            {
                throw new NoDiffSolutionException();
            }

            var snakes = new List<Snake>();
            var p = new LongPoint(n, m);
            for (var d = vs.Count - 1; p.X > 0 || p.Y > 0; d--)
            {
                var snake = Snake.CalculateSnake(vs[d], p.X - p.Y, d, a, n, b, m);
                snakes.Add(snake);

                p.X = snake.Start.X;
                p.Y = snake.Start.Y;
            }

            return SnakesToOperations(a, b, snakes);
        }

        /// <summary>
        /// Преобразование цепочки змеек в список операций
        /// </summary>
        /// <param name="a">Оригинал</param>
        /// <param name="b">Источник с изменениями</param>
        /// <param name="snakes">Цепочка змеек с решением</param>
        /// <returns>Список операций</returns>
        private static IEnumerable<Operation> SnakesToOperations(ISource a, ISource b, IEnumerable<Snake> snakes)
        {
            var operations = new List<Operation>();
            foreach (var snake in snakes)
            {
                /*
                 * Если мы не имеем дело с равенством, то добавим операцию изменения
                 */
                if (snake.OperationKind != OperationKind.Equal)
                {
                    /*
                     * Преобразуем начало змейки в операцию:
                     *      Если мы имеем дело с удлаением, то возьмем в качестве источника для операции оригинал а, а позицией будет координата элемента по оси Х.
                     *      В противном случае мы возьмем в качестве источника источник с изменениями b и позицией в этом случае будет координата элемента по оси Y.
                     */
                    operations.Add(new Operation(
                        snake.OperationKind,
                        snake.OperationKind == OperationKind.Delete ? snake.Start.X : (snake.Start.Y == -1 ? 0 : snake.Start.Y),
                        snake.OperationKind == OperationKind.Delete ? a : b
                        ));
                }
                /*
                 * Диагональ в змейке означает, что движение по графу идет со смещением по осям Х и Y одновременно.
                 * Согласно Майерсу, это означает совпадение элементов в обоих источниках.
                 */
                if (snake.Diagonals == 0) continue;
                for (var i = 0; i < snake.Diagonals; i++)
                {
                    /*
                     * Позиция операции высчитывается как смещение по оси Х, равное текущему шагу по диагонали.
                     * В случае если операция была удалением, то необходимо добавить 1 к координате Х.
                     */
                    operations.Add(new Operation(
                                       OperationKind.Equal,
                                       snake.Start.X + (snake.OperationKind == OperationKind.Delete ? 1 : 0) + i,
                                       a));
                }
            }
            return operations;
        }
    }
}