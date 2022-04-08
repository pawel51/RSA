using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RSA
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StringToAsciiConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            
            string msg = (string)value;
            int[] chars = new int[msg.Length];
            for (int i = 0; i < msg.Length; i++)
            {
                chars[i] = msg[i];
            }
            return "(" + String.Join(", ", chars) + ")";


        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
