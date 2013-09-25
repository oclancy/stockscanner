using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StockScanner.UI.Converters
{
    public class FinancialDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value.GetType() != typeof(double)) return value;

            var val = (double)value;
            if (val / 1000000000 > 1)
                return string.Format("{0}b", val / 1000000000);
            if (val / 1000000 > 1)
                return string.Format("{0}m", val / 1000000);
            if (val / 1000 > 1)
                return string.Format("{0}k", val / 1000);

            if(parameter.ToString().EndsWith("Ratio"))
                return string.Format("{0}%", val);

            return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
