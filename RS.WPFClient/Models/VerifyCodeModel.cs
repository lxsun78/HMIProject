using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RS.WPFClient.Models
{
    public class VerifyCodeModel: ObservableObject
    {

        private bool isFocused;
        /// <summary>
        /// 是否聚焦
        /// </summary>
        public bool IsFocused
        {
            get { return isFocused; }
            set
            {
                this.SetProperty(ref isFocused, value);
            }
        }

        private string text;
        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                this.SetProperty(ref text, value);
            }
        }

        private int caretIndex;
        /// <summary>
        /// 光标位置
        /// </summary>
        public int CaretIndex
        {
            get { return caretIndex; }
            set
            {
                this.SetProperty(ref caretIndex, value);
            }
        }

        public ICommand VerifyCodeKeyDownCommand { get; set; }
        public ICommand VerifyCodeTextChangedCommand { get; set; }

    }
}
