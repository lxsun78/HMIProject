using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Widgets.Models
{
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
    }
}
