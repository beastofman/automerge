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

            /*
             * Если один из источников пуст, то
             * построим список операций вставки для другого.
             *
             * Если оба пусты, то обработку такой ситуации оставим на усмотрение вызывающего сервиса, вернув null.
             */
            if (a == null ||
                b == null)
            {
                return BuildOperationsForSingleSource(a) ?? BuildOperationsForSingleSource(b);
            }

            var n = a.Length;
            var m = b.Length;

            var vs = new List<VArray>();
            var v = new VArray(n, m) { [1] = 0 };
            var hasSolution = false;
            /*
             * d - длина наименьшего списка изменений, необходимого для превращения А в В.
             * Не зная решения заранее, мы подразумеваем, что при самом неблагоприятном исходе
             * d будет равно сумме длин А и В, т.е. В полностью будет отличаться от А.
             *
             * Соответственно, D-путь - путь с началом в точке (0;0), включающий ровно D
             * недиагональных (операция вставки или удаления) шагов, в резльтате которого А можно превратить в В.
             */
            for (var d = 0; d < n + m; d++)
            {
                /*
                 * Согласно Майерсу, каждый D-путь должен оканчиваться на диагонали k (k = x - y), k ∈ {-D ,-D+2, ..., D-2, D}
                 */
                for (var k = -d; k <= d; k += 2)
                {
                    /*
                     * Найдем направление движения в матрице.
                     * Если мы оказались на диагонали k = -d или ушли с текущей диагонали и следующий шаг вниз (вставка нового элемента) будет выгоднее и в итоге мы дальше продвинемся по диагонали k.
                     */
                    var down = (k == -d || (k != d && v[k - 1] < v[k + 1]));

                    //Начало змейки, координата Х
                    var xStart = down ? v[k + 1] : v[k - 1];
                    /*
                     * Конец змейки, координата Х.
                     * Если мы идем вправо (удаление элемента), то смещаемся по оси Х на 1 шаг относительно начала.
                     */
                    var xEnd = xStart + (down ? 0 : 1);
                    //Координату Y вычисляем по формуле y = x - k
                    var yEnd = xEnd - k;
                    /*
                     * Мы определили операцию (вставку или удаление),
                     * далее найдем количество совпадающих элементов - диагональных шагов в змейке.
                     * Делаем это до тех пор, пока не дойдем в нижнюю правую точку матрицы или не закончатся совпадения элементов.
                     */
                    while (xEnd < n &&
                           yEnd < m &&
                           Equals(a[xEnd], b[yEnd]))
                    {
                        xEnd++;
                        yEnd++;
                    }
                    //Сохраним максимально длинный путь по диагонали k в массив V
                    v[k] = xEnd;

                    //Продолжаем, если мы не дошли до нижней правой точки матрицы
                    if (xEnd < n ||
                        yEnd < m) continue;

                    //Есть решение!
                    hasSolution = true;
                    break;
                }

                //Добавим текущий массив V в список для дальнейшего построения змеек.
                vs.Add(v.Copy(d));
                /*
                 * Остановим поиски, если решение найдено.
                 * Особенность алгоритма в том, что он останавливает работу когда будет найдено первое решение
                 */
                if (hasSolution) break;
            }

            if (!hasSolution)
            {
                throw new NoDiffSolutionException();
            }

            var snakes = new List<Snake>();
            var p = new LongPoint(n, m);
            /*
             * Переберем все возможные варианты движения из сохраненных на этапе работы алгоритма.
             * Т.к. змейки строятся из конечной точки в начальную (из правого нижнего угла матрицы в верхний левый),
             * то мы либо переберем все возможные массивы V, либо дойдем до точки начала и на том остановимся.
             */
            for (var d = vs.Count - 1; p.X > 0 || p.Y > 0; d--)
            {
                //Построим змейку на основании текущего массива V
                var snake = Snake.CalculateSnake(vs[d], p.X - p.Y, d, a, n, b, m);
                //Сохраним змейку
                snakes.Add(snake);

                //Переместимся в точку, где оканчивается текущая змейка
                p.X = snake.Start.X;
                p.Y = snake.Start.Y;
            }
            snakes.Reverse();
            return SnakesToOperations(a, b, snakes);
        }

        /// <summary>
        /// Получить список операций для источника
        /// </summary>
        /// <param name="source">Источник</param>
        /// <returns></returns>
        private static IEnumerable<Operation> BuildOperationsForSingleSource(ISource source)
        {
            if (source == null) return null;
            var operations = new List<Operation>();
            for (var i = 0; i < source.Length; i++)
            {
                operations.Add(new Operation(OperationKind.Insert, i, source));
            }
            return operations;
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