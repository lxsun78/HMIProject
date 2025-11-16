using RS.WPFClient.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RS.WPFClient.Converters
{
    public class ReadUnReadTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var readUnReadType = (ReadUnReadType)value;
            // 语言本地化待接入
            string description = "不限";
            switch (readUnReadType)
            {
                case ReadUnReadType.Any:
                    description = "不限";
                    break;
                case ReadUnReadType.UnRead:
                    description = "未读";
                    break;
                case ReadUnReadType.Read:
                    description = "已读";
                    break;
            }
            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}


