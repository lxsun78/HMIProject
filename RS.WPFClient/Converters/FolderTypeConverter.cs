using RS.WPFClient.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RS.WPFClient.Converters
{
    public class FolderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var folderType = (FolderType)value;
            // 语言本地化待接入
            string description = "不限";
            switch (folderType)
            {
                case FolderType.Any:
                    description = "不限";
                    break;
                case FolderType.Inbox:
                    description = "收件箱";
                    break;
                case FolderType.Sent:
                    description = "已发送";
                    break;
                case FolderType.Drafts:
                    description = "草稿箱";
                    break;
                case FolderType.Deleted:
                    description = "已删除";
                    break;
                case FolderType.GroupMail:
                    description = "群邮件";
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


