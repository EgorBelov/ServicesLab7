using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace FahrenheitToCelsiusConversion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CurrencyConversionService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CurrencyConversionService.svc or CurrencyConversionService.svc.cs at the Solution Explorer and start debugging.
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private const string ExchangeRatesUrl = "https://www.cbr.ru/scripts/XML_daily.asp";

        public decimal ConvertCurrency(decimal amount, string sourceCurrency, string targetCurrency)
        {
            var exchangeRate = GetExchangeRate(sourceCurrency, targetCurrency);
            if (exchangeRate != null)
            {
                return amount * exchangeRate.Rate;
            }

            throw new ArgumentException("Invalid currency code.");
        }

        private CurrencyRate GetExchangeRate(string sourceCurrency, string targetCurrency)
        {
            var xml = LoadExchangeRatesXml();

            var sourceRate = FindCurrencyRate(xml, sourceCurrency);
            var targetRate = FindCurrencyRate(xml, targetCurrency);
            if (sourceCurrency == "RUB" || targetCurrency == "RUB")
            {
                if (sourceCurrency != targetCurrency && sourceCurrency == "RUB")
                {
                    var currencyElement = xml.Descendants("Valute")
                        .FirstOrDefault(e => e.Element("CharCode")?.Value == targetCurrency.ToUpper());
                    return new CurrencyRate { CurrencyCode = targetCurrency, Rate = 1 / decimal.Parse(currencyElement.Element("Value")?.Value) };
                }
                else
                {
                    var currencyElement = xml.Descendants("Valute")
                        .FirstOrDefault(e => e.Element("CharCode")?.Value == targetCurrency.ToUpper());
                    return new CurrencyRate { CurrencyCode = targetCurrency, Rate =  decimal.Parse(currencyElement.Element("Value")?.Value) };
                }
            }
            if (sourceRate != null && targetRate != null)
            {
                var rate = sourceRate.Rate / targetRate.Rate;
                return new CurrencyRate { CurrencyCode = targetCurrency, Rate = rate };
            }
            
            return null;
        }

        private XDocument LoadExchangeRatesXml()
        {
            using (var client = new HttpClient())
            {
                var xmlString = client.GetStringAsync(ExchangeRatesUrl).Result;
                return XDocument.Parse(xmlString);
            }
        }

        private CurrencyRate FindCurrencyRate(XDocument xml, string currencyCode)
        {
            var currencyElement = xml.Descendants("Valute")
                .FirstOrDefault(e => e.Element("CharCode")?.Value == currencyCode.ToUpper());

            if (currencyElement != null)
            {
                var rate = currencyElement.Element("Value")?.Value;
                if (decimal.TryParse(rate, out decimal decimalRate))
                {
                    return new CurrencyRate { CurrencyCode = currencyCode, Rate = decimalRate };
                }
            }

            return null;
        }
        public async Task<List<CurrencyRate>> GetAvailableCurrencies()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(ExchangeRatesUrl);
                var content = await response.Content.ReadAsStringAsync();
                var currencyData = JsonConvert.DeserializeObject<CurrencyData>(content);
                return currencyData.Valute.Values.Select(currencyRate => new CurrencyRate
                {
                    CurrencyCode = currencyRate.CurrencyCode,
                    Rate = currencyRate.Rate
                }).ToList();
            }
        }
    }
    public class CurrencyData
    {
        public Dictionary<string, CurrencyRate> Valute { get; set; }
    }
}
