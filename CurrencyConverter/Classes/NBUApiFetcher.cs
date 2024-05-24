using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CurrencyConverter
{
    internal static class NBUApiFetcher
    {
        public static List<Currency> fetchedCurrencies = new List<Currency>();


        public static async Task FetchCurrenciesAsync()
        {
            string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string jsonResponse = await client.GetStringAsync(url);
                    List<Currency> mfetchedCurrencies = JsonConvert.DeserializeObject<List<Currency>>(jsonResponse);

                    if (mfetchedCurrencies != null)
                    {
                        fetchedCurrencies = mfetchedCurrencies;
                        fetchedCurrencies.Add(new Currency("UAH", 1));
                        fetchedCurrencies.Add(new Currency("--SELECT--", 0));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        public static async Task FetchCurrenciesAsyncOnDate(string date)
        {
            string url = $"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={date}&json";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string jsonResponse = await client.GetStringAsync(url);
                    List<Currency> mfetchedCurrencies = JsonConvert.DeserializeObject<List<Currency>>(jsonResponse);

                    if (mfetchedCurrencies.Count>=1)
                    {
                        fetchedCurrencies = mfetchedCurrencies;
                        fetchedCurrencies.Add(new Currency("UAH", 1));
                        fetchedCurrencies.Add(new Currency("--SELECT--", 0));
                    }
                    else
                    {
                        fetchedCurrencies.Clear();
                        fetchedCurrencies.Add(new Currency("WRONG DATE", 1));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

    }
}
