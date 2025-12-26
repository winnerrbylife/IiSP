using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IPR1_2.Models;

namespace IPR1_2.Services
{
    public class RateService : IRateService
    {
        private readonly HttpClient _httpClient;
        
        private readonly Dictionary<string, string> _currencies = new Dictionary<string, string>
        {
            { "RUB", "RUB" },
            { "EUR", "EUR" },
            { "USD", "USD" },
            { "CHF", "CHF" },
            { "CNY", "CNY" },
            { "GBP", "GBP" }
        };

        public RateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Rate>> GetDailyRates(DateTime date)
        {
            var rates = new List<Rate>();
            string formattedDate = date.ToString("yyyy-MM-dd");

            foreach (var currency in _currencies)
            {
                try
                {
                    string url = $"rates/{currency.Value}?ondate={formattedDate}&periodicity=0&parammode=2";
                    
                    var response = await _httpClient.GetAsync(url);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        
                        Console.WriteLine($"Response for {currency.Key}: {json}");
                        
                        var rate = JsonSerializer.Deserialize<Rate>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = false
                        });
                        
                        if (rate != null)
                        {
                            rates.Add(rate);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error {response.StatusCode} for currency {currency.Key}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading rate {currency.Key}: {ex.Message}");
                }
            }

            return rates;
        }
    }
}