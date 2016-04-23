using System;

namespace Domain
{
    /// <summary>
    /// Интерфейс источника данных для сравнения
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Общее количество элементов в источнике
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Элемент источника для сравнения
        /// </summary>
        /// <param name="index">Индекс элемента</param>
        /// <returns>IComparable для сравнения с другим элементом</returns>
        IComparable this[int index] { get; }
    }
}