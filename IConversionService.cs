using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FahrenheitToCelsiusConversion
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IConversionService" in both code and config file together.
    [ServiceContract]
    public interface IConversionService
    {
        [OperationContract]
        double FahrenheitToCelsius(double farenheitDegrees);

        [OperationContract]
        double CelsiusToFahrenheit(double celsiusDegrees);

     
    }
}
