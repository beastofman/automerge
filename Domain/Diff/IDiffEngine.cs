using System.Collections.Generic;

namespace Domain
{
    /// <summary>
    /// Интерфейс для поиска различий между двумя источниками
    /// </summary>
    public interface IDiffEngine
    {
        /// <summary>
        /// Получить список операций, при помощи которых можно превратить original в target
        /// </summary>
        /// <param name="original">Оригинал</param>
        /// <param name="target">Источник с изменениями</param>
        /// <returns>Список операций</returns>
        IEnumerable<Operation> GetDiff(ISource original, ISource target);
    }
}