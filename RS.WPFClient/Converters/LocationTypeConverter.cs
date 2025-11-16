using RS.WPFClient.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RS.WPFClient.Converters
{
    public class LocationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var locationType = (LocationType)value;
            // 语言本地化待接入
            string description = "不限";
            switch (locationType)
            {
                case LocationType.Any:
                    description = "不限";
                    break;
                case LocationType.Subject:
                    description = "主题";
                    break;
                case LocationType.EmailBody:
                    description = "邮件正文";
                    break;
                case LocationType.AttachmentName:
                    description = "附件名称";
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


