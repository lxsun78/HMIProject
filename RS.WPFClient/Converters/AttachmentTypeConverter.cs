using RS.WPFClient.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RS.WPFClient.Converters
{
    public class AttachmentTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var attachmentType = (AttachmentType)value;
            // 语言本地化待接入
            string description = "不限";
            switch (attachmentType)
            {
                case AttachmentType.Any:
                    description = "不限";
                    break;
                case AttachmentType.IncludeAttachment:
                    description = "包含附件";
                    break;
                case AttachmentType.NotIncludeAttachment:
                    description = "不包含附件";
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


