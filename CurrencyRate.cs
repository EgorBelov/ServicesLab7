using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FahrenheitToCelsiusConversion
{
    [DataContract]
    public class CurrencyRate
    {
        [DataMember]
        public string CurrencyCode { get; set; }

        [DataMember]
        public decimal Rate { get; set; }
    }


}