using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Клас який описує валюти
namespace CurrencyConverter
{
    internal class Currency
    {
        [JsonProperty("rate")] //Інструкція для Json, щоб сереалізував отриману дату з даним полем
        public double rate;
        [JsonProperty("cc")]
        public string currencyCode;

        public Currency(string cc, double rate)
        {
            this.rate = rate;
            this.currencyCode = cc;
        }
    }
}
