using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Widgets.Models
{
    /// <summary>
    /// 标签数据项来源类型
    /// </summary>
    public enum TagItemSource
    {
        /// <summary>
        /// 来自数据源（ItemsSource）
        /// </summary>
        FromItemsSource = 0,

        /// <summary>
        /// 用户自定义输入
        /// </summary>
        Custom = 1
    }

    public class TagModel : ObservableObject
    {
        private object? tagContent;
        /// <summary>
        /// 标签内容
        /// </summary>
        public object? TagContent
        {
            get { return tagContent; }
            set
            {
                this.SetProperty(ref tagContent, value);
            }
        }

        private bool isSelect;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelect
        {
            get { return isSelect; }
            set
            {
                this.SetProperty(ref isSelect, value);
            }
        }

        private object? data;
        /// <summary>
        /// 关联数据
        /// </summary>
        public object? Data
        {
            get { return data; }
            set
            {
                this.SetProperty(ref data, value);
            }
        }

        private TagItemSource source;
        /// <summary>
        /// 数据来源类型
        /// </summary>
        public TagItemSource Source
        {
            get { return source; }
            set
            {
                this.SetProperty(ref source, value);
            }
        }

        /// <summary>
        /// Equals 和 GetHashCode 只基于 Data，这样 TagModel 可以用于 HashSet
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is TagModel other)
            {
                return ReferenceEquals(Data, other.Data) || (Data?.Equals(other.Data) ?? false);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Data?.GetHashCode() ?? 0;
        }
    }
}
