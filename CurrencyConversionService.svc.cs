using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

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

            if (sourceRate != null && targetRate != null)
            {
                var rate = targetRate.Rate / sourceRate.Rate;
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
    }
}
