using System.Globalization;

namespace ElProgreso.UI;

/// <summary>
/// Formatting helpers so amounts and dates look the way a Colombian teller expects them.
/// </summary>
internal static class ConsoleFormat
{
    private static readonly CultureInfo Cop = CultureInfo.GetCultureInfo("es-CO");
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public static string Cop0(decimal amount) => amount.ToString("C0", Cop);

    public static string Usd2(decimal amount) => amount.ToString("C2", Usd);

    public static string ShortDate(DateTime date) => date.ToString("dd/MM/yyyy", Cop);

    public static string DateTimeStamp(DateTime date) => date.ToString("dd/MM/yyyy HH:mm", Cop);
}