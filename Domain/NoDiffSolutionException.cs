using System;

namespace Domain
{
    /// <summary>
    /// Исключение для ситуации, когда не может быть найдена разница между двумя источниками
    /// </summary>
    public sealed class NoDiffSolutionException : Exception
    {
    }
}