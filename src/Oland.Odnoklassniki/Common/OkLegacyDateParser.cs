using System.Globalization;

namespace Oland.Odnoklassniki.Common;

/// <summary>
/// Разбирает резервное поле <c>date</c> ("yyyy-MM-dd HH:mm:ss", московское время без часового пояса
/// в самой строке), которое OK отдаёт вместо <c>created_ms</c>/<c>date_ms</c> для некоторых типов
/// обсуждений (замечено на личных <c>USER_STATUS</c>/<c>USER_PHOTO</c> — там числовые поля времени
/// приходят как <c>null</c>). Москва не переходит на летнее время с 2014 года, поэтому фиксированный
/// офсет UTC+3 корректен всегда, без TimeZoneInfo/DST-логики.
/// </summary>
internal static class OkLegacyDateParser
{
    private const string Format = "yyyy-MM-dd HH:mm:ss";
    private static readonly TimeSpan MoscowOffset = TimeSpan.FromHours(3);

    /// <summary>Возвращает Unix-время в мс, либо 0, если строка отсутствует/не разбирается.</summary>
    public static long ParseToUnixMs(string? date)
    {
        if (string.IsNullOrWhiteSpace(date)) return 0;

        if (!DateTime.TryParseExact(date, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var moscowLocal))
            return 0;

        var utc = DateTime.SpecifyKind(moscowLocal - MoscowOffset, DateTimeKind.Utc);
        return new DateTimeOffset(utc).ToUnixTimeMilliseconds();
    }
}
