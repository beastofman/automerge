namespace Domain
{
    /// <summary>
    /// Тип операции
    /// </summary>
    public enum OperationKind
    {
        /// <summary>
        /// Равенство элементов
        /// </summary>
        Equal = 0,

        /// <summary>
        /// Удаление элемента
        /// </summary>
        Delete = 1,

        /// <summary>
        /// Добавление нового элемента
        /// </summary>
        Insert = 2,

        /// <summary>
        /// Неизвестная операция
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Операция, производимая над источником
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Позиция операции в источнике
        /// </summary>
        public long Index { get; private set; }

        /// <summary>
        /// Признак того, что данная операция — конфликт.
        /// </summary>
        public bool IsConflict { get; set; }

        /// <summary>
        /// Тип операции
        /// </summary>
        public OperationKind Kind { get; private set; }

        /// <summary>
        /// Источник, из которого взята операция
        /// </summary>
        public ISource Source { get; private set; }

        public Operation(OperationKind kind, long index, ISource source)
        {
            this.Kind = kind;
            this.Index = index;
            this.Source = source;
            this.IsConflict = false;
        }
    }
}