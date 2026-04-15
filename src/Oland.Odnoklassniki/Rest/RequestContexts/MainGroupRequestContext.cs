using Microsoft.Extensions.Options;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.RequestContexts;

/// <summary>
/// Контекст выполнения запроса от имени основной учётной записи, привязанный к конкретной группе.
/// </summary>
/// <remarks>
/// <para>
/// Наследует <see cref="MainAccountRequestContext"/> и добавляет явную привязку к идентификатору группы
/// через свойство <see cref="GroupId"/>.
/// </para>
/// <para>
/// <b>Область применения:</b> API-методы, выполняемые с правами владельца/администратора приложения,
/// но оперирующие данными в контексте конкретной группы (например, управление каталогами или товарами группы).
/// </para>
/// <para>
/// <b>Инициализация:</b> параметры извлекаются из конфигурации через <see cref="IOptions{TOptions}"/>.
/// Идентификатор группы считывается из <c>ApplicationOptions.GroupId</c> и оборачивается в <see cref="GroupId"/>.
/// </para>
/// </remarks>
public record MainGroupRequestContext : MainAccountRequestContext
{
    /// <summary>
    /// Идентификатор целевой группы, в контексте которой выполняется запрос.
    /// </summary>
    /// <remarks>
    /// Инициализируется один раз при создании экземпляра из конфигурации.
    /// </remarks>
    public GroupId GroupId { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainGroupRequestContext"/> с использованием настроек приложения.
    /// </summary>
    /// <param name="Options">
    /// Настройки приложения, содержащие идентификатор группы (<c>ApplicationOptions.GroupId</c>)
    /// и параметры авторизации основной учётной записи.
    /// </param>
    /// <remarks>
    /// Вызывает базовый конструктор <see cref="MainAccountRequestContext"/> для инициализации токенов доступа.
    /// Идентификатор группы извлекается из опций и оборачивается в типобезопасный <see cref="GroupId"/>.
    /// </remarks>
    public MainGroupRequestContext(IOptions<ApplicationOptions> Options) : base(Options)
    {
        GroupId = new GroupId(Options.Value.GroupId);
    }
}