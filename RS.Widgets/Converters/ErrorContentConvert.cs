using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace RS.Widgets.Converters
{

    public class MultiErrorContentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> errorList = new List<string>();
            string errorMsg = string.Empty;
            foreach (var item in values)
            {
                if (item is ReadOnlyObservableCollection<ValidationError> errors && errors.Count > 0)
                {
                    errorList = errorList.Concat(errors.Select(e => e.ErrorContent?.ToString() ?? "")).ToList();
                }
            }

            if (errorList.Count>0)
            {
                return string.Join(Environment.NewLine, errorList);
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
