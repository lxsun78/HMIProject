using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RS.Widgets.Converters
{

    /// <summary>
    /// 支持任意数量元素的错误信息转换器
    /// 接收一个UI元素集合，提取每个元素的Validation.Errors并合并为多行文本
    /// </summary>
    public class MultiElementErrorConverter : IValueConverter
    {
        /// <summary>
        /// 转换逻辑：遍历元素集合，收集所有错误信息
        /// </summary>
        /// <param name="value">UI元素集合（如List<FrameworkElement>）</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">可选：错误前缀（如"邮箱："、"密码："，用逗号分隔与元素对应）</param>
        /// <param name="culture"></param>
        /// <returns>合并后的错误文本（一行一个错误）</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 校验输入是否为元素集合
            if (value is not IEnumerable<FrameworkElement> elements)
                return string.Empty;

            var allErrors = new List<string>();
            // 解析参数（可选：每个元素对应的错误前缀，如"邮箱,密码,姓名"）
            var prefixes = parameter?.ToString()?.Split(',') ?? Array.Empty<string>();
            int index = 0;

            foreach (var element in elements)
            {
                // 获取当前元素的Validation.Errors
                var errors = Validation.GetErrors(element);
                if (errors.Count == 0)
                {
                    index++;
                    continue;
                }

                // 获取当前元素的前缀（参数不足则用默认值）
                string prefix = index < prefixes.Length && !string.IsNullOrEmpty(prefixes[index])
                    ? $"{prefixes[index]}："
                    : string.Empty;

                // 收集错误信息（带前缀）
                allErrors.AddRange(errors.Select(e => $"{prefix}{e.ErrorContent}"));
                index++;
            }

            // 用换行符拼接所有错误
            return string.Join(Environment.NewLine, allErrors);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
