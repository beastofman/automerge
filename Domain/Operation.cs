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
        public int Index { get; }

        /// <summary>
        /// Признак того, что данная операция — конфликт.
        /// </summary>
        public bool IsConflict { get; set; }

        /// <summary>
        /// Тип операции
        /// </summary>
        public OperationKind Kind { get; }

        /// <summary>
        /// Источник, из которого взята операция
        /// </summary>
        public ISource Source { get; }

        public Operation(OperationKind kind, int index, ISource source, bool isConflict = false)
        {
            this.Kind = kind;
            this.Index = index;
            this.Source = source;
            this.IsConflict = isConflict;
        }

        public override bool Equals(object obj)
        {
            var op = obj as Operation;
            if (op == null) return false;
            return op.Kind == this.Kind &&
                   op.Index == this.Index &&
                   op.IsConflict == this.IsConflict &&
                   op.Source == this.Source;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Index.GetHashCode();
                hashCode = (hashCode * 397) ^ this.IsConflict.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.Kind;
                hashCode = (hashCode * 397) ^ (this.Source?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{(this.IsConflict ? "! " : string.Empty)}{this.Kind} Index: {this.Index} Source: {this.Source}";
        }

        protected bool Equals(Operation other)
        {
            return this.Index == other.Index && this.IsConflict == other.IsConflict && this.Kind == other.Kind && ReferenceEquals(this.Source, other.Source);
        }
    }
}