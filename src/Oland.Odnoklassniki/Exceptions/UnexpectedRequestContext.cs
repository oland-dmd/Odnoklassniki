using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Exceptions;

/// <summary>
/// Исключение, возникающее при передаче неподдерживаемого типа контекста запроса.
/// </summary>
public class UnexpectedRequestContext : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр исключения с указанием текущего и ожидаемых типов контекста.
    /// </summary>
    /// <param name="currentContext">Фактический переданный контекст запроса.</param>
    /// <param name="expectedTypes">Наименования типов контекста, которые являются допустимыми.</param>
    public UnexpectedRequestContext(IRequestContext currentContext, params string[] expectedTypes)
        : base($"{currentContext.GetType().Name} is not supported request context. Expected contexts: {string.Join(',', expectedTypes)}")
    {
    }
}