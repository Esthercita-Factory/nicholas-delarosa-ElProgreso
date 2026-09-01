using System.Globalization;

namespace ElProgreso.UI;

/// <summary>
/// Reads and validates keyboard input, reprompting the teller instead of crashing on bad input.
/// </summary>
internal static class ConsoleReader
{
    public static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("Este dato es obligatorio, intente de nuevo.");
        }
    }

    public static string? ReadOptionalText(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
    }

    public static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ||
                decimal.TryParse(input, NumberStyles.Number, CultureInfo.GetCultureInfo("es-CO"), out value))
            {
                return value;
            }

            Console.WriteLine("Ese valor no es un número válido, intente de nuevo.");
        }
    }

    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ||
                DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }

            Console.WriteLine("Use el formato dd/mm/aaaa, por ejemplo 15/03/2026.");
        }
    }

    public static int ReadMenuOption(int min, int max)
    {
        while (true)
        {
            Console.Write("Seleccione una opción: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var option) && option >= min && option <= max)
            {
                return option;
            }

            Console.WriteLine($"Ingrese un número entre {min} y {max}.");
        }
    }

    public static bool ReadConfirmation(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (s/n): ");
            var input = Console.ReadLine()?.Trim().ToLowerInvariant();
            switch (input)
            {
                case "s":
                case "si":
                case "sí":
                    return true;
                case "n":
                case "no":
                case "nah":
                    return false;
                default:
                    Console.WriteLine("Responda 's' o 'n'.");
                    break;
            }
        }
    }
}