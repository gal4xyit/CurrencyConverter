using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json; //Завантажений NuGet пакет для роботи із Json файлами

namespace CurrencyConverter
{
    internal static class NBUApiFetcher
    {
        public static List<Currency> fetchedCurrencies = new List<Currency>(); //Створення Листа для зберігання отриманих данних

        //Метод для отримання данних за сьогоднішній день
        public static async Task FetchCurrenciesAsync()
        {
            string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json"; //Посилання на API НБУ

            //Процес витягу данних
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string jsonResponse = await client.GetStringAsync(url);
                    List<Currency> mfetchedCurrencies = JsonConvert.DeserializeObject<List<Currency>>(jsonResponse);

                    if (mfetchedCurrencies.Count >= 1)//Перевірка чи данні існують
                    {
                        //Присвоєння данних
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

        //Метод для отримання данних за вибрану дату
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
                        // Якщо данних на дану дату нема то повертається інформацію для користувача про помилку
                        fetchedCurrencies.Clear();
                        fetchedCurrencies.Add(new Currency("WRONG DATE", 0));
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
