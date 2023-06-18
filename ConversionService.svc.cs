using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FahrenheitToCelsiusConversion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ConversionService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ConversionService.svc or ConversionService.svc.cs at the Solution Explorer and start debugging.
    public class ConversionService : IConversionService
    {
        public double FahrenheitToCelsius(double farenheitDegrees)
        {
            return 5.0 / 9 * (farenheitDegrees - 32);
        }

        public double CelsiusToFahrenheit(double celsiusDegrees)
        {
            return 9.0 / 5 * celsiusDegrees + 32;
        }

       
    }
}


