using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElProgreso.Services;

/// <summary>
/// Official peso-to-dolar exchange rate (TRM), as published by the Colombian Superintendencia Financiera (SFC) 
/// </summary>
/// <param name="Value"></param>
/// <param name="ValidFrom"></param>
/// <param name="ValidUntil"></param>
public sealed record ExchangeRate(decimal Value, DateTime ValidFrom, DateTime ValidUntil);

/// <summary>
/// Reads the lastest TRM from the government's open-data portal.
/// Never throws: any network, timeout or parsing failure is swallowed and reported as "unavailable" so a slow or unreachable API never takes the rest of the application down with it.
/// </summary>
/// <param name="httpClient"></param>
public sealed class ExchangeRateService(HttpClient httpClient)
{
    private const string RequestUri = "resource/32sa-8pi3.json?$order=vigenciadesde%20DESC&$limit=1";

    public async Task<ExchangeRate?> GetLastestRateAsync()
    {
        try
        {
            var records = await httpClient.GetFromJsonAsync<List<TrmRecord>>(RequestUri);
            var record = records?.FirstOrDefault();

            if (record?.Value is null || record.ValidFrom is null || record.ValidUntil is null)
            {
                return null;
            }

            if (!decimal.TryParse(record.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate))
            {
                return null;
            }

            return new ExchangeRate(rate, record.ValidFrom.Value, record.ValidUntil.Value);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            return null;
        }
    }
    
    private sealed class TrmRecord
    {
        [JsonPropertyName("valor")]
        public string? Value { get; set; }
        
        [JsonPropertyName("vigenciadesde")]
        public DateTime? ValidFrom { get; set; }
        
        [JsonPropertyName("vigenciahasta")]
        public DateTime? ValidUntil { get; set; }
    }
}