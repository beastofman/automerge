namespace Domain
{
    /// <summary>
    /// Стратегия для автоматического разрешения конфликтов при обнаружении совпадающих операций
    /// </summary>
    public struct ResolveStrategy
    {
        /// <summary>
        /// Действие
        /// </summary>
        public ResolveStrategyAction Action { get; }

        /// <summary>
        /// Признак отметки операции как конфликтной
        /// </summary>
        public bool MarkAsConflict { get; }

        public ResolveStrategy(ResolveStrategyAction action, bool markAsConflict)
        {
            this.Action = action;
            this.MarkAsConflict = markAsConflict;
        }
    }

    /// <summary>
    /// Действие при разрешении конфликта
    /// </summary>
    public enum ResolveStrategyAction
    {
        /// <summary>
        /// Не принимать изменения в случае конфликта
        /// </summary>
        Ignore,

        /// <summary>
        /// Принимать изменения из всех источников
        /// </summary>
        AcceptAll,

        /// <summary>
        /// Принимать изменения только из первого источника
        /// </summary>
        AcceptFirst,

        /// <summary>
        /// Принимать изменения только из второго источника
        /// </summary>
        AcceptSecond
    }
}