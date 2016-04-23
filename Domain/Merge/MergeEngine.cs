using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    /// <summary>
    /// Движок, выполняющий слияние двух списков изменений для получения финального списка
    /// </summary>
    public class MergeEngine
    {
        /// <summary>
        /// Список совпадающих операций, которые можно рассматривать как конфликтные и каким образом разрешать конфликт
        /// </summary>
        public IDictionary<OperationKind, ResolveStrategy> ConflictOperationsResolveStrategies { get; }

        /// <summary>
        /// Стратегия по умолчанию для разрешения конфликтов, если описание не найдено в ConflictOperationsResolveStrategies
        /// </summary>
        public ResolveStrategy DefaultResolveStrategy { get; }

        public MergeEngine(ResolveStrategy defaultResolveStrategy)
        {
            this.DefaultResolveStrategy = defaultResolveStrategy;
            this.ConflictOperationsResolveStrategies = new Dictionary<OperationKind, ResolveStrategy>();
        }

        /// <summary>
        /// Получение итогового списка операций
        /// </summary>
        /// <param name="operations1">Первый список операций</param>
        /// <param name="operations2">Второй список операций</param>
        /// <returns></returns>
        public virtual IEnumerable<Operation> GetMergeOperations(IList<Operation> operations1, IList<Operation> operations2)
        {
            /*
             * Если хоть один из списков пустой,
             * то однозначно берем операции из другого.
             *
             * Если оба пусты, то беда :)
             */
            if (operations1 == null ||
                operations2 == null)
            {
                return operations1 ?? operations2;
            }

            if (!operations1.Any() ||
                !operations2.Any())
            {
                return operations1.Any() ? operations1 : operations2;
            }

            /*
             * Перво-наперво найдем максимально длинный список операций,
             * чтобы от этого значения можно было отталкиваться при переборе операций.
             */
            var max = Math.Max(operations1.Count, operations2.Count);
            var final = new List<Operation>();

            for (var i = 0; i < max; i++)
            {
                /*
                 * Найдем операции в обоих списках на текущей позиции.
                 * Критерий отсутсвия операции - превышение текущей позицией длины любого из списков.
                 */
                var op1 = i < operations1.Count ? operations1[i] : null;
                var op2 = i < operations2.Count ? operations2[i] : null;

                /*
                 * Проверяем наличие операций в обоих списках
                 */
                if (op1 == null ||
                    op2 == null)
                {
                    /*
                     * Если в одном из списков операция отсутствует,
                     * то берем из другого.
                     */
                    final.Add(op1 ?? op2);
                }
                else
                {
                    /*
                     * В обоих списках на текущей позиции присутствует операция.
                     * Если операции не совпадают, то
                     */
                    if (op1.Kind != op2.Kind)
                    {
                        //Вычисляем приоритет операций друг относительно друга.
                        final.Add(op1.Kind > op2.Kind ? op1 : op2);
                    }
                    else
                    {
                        /*
                         * В противном случае обратимся к стратегиям принятия решения при конфликте.
                         * Если текущей операции нет в списке стратегий, то возьмем стратегию по умолчанию.
                         */

                        var strategy = !this.ConflictOperationsResolveStrategies.ContainsKey(op1.Kind)
                            ? this.DefaultResolveStrategy
                            : this.ConflictOperationsResolveStrategies[op1.Kind];
                        /*
                         * Помечать ли текущую ситуацию как конфликтную или нет зависит от того,
                         * есть ли в выбранной стратегии соответствующий флаг.
                         */
                        op1.IsConflict = op2.IsConflict = strategy.MarkAsConflict;

                        /*
                         * Далее смотрим что делать
                         */
                        switch (strategy.Action)
                        {
                            case ResolveStrategyAction.AcceptAll:
                                //Берем обе
                                final.Add(op1);
                                final.Add(op2);
                                break;

                            case ResolveStrategyAction.AcceptFirst:
                                //Берем только первую
                                final.Add(op1);
                                break;

                            case ResolveStrategyAction.AcceptSecond:
                                //Берем только вторую
                                final.Add(op2);
                                break;

                                /* Остальные варианты либо сугубо служебные (MarkAsConflict),
                                 * либо подразумевают, что обе операции мы проигнорируем (Ignore)
                                 */
                        }
                    }
                }
            }
            return final;
        }
    }
}