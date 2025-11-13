using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RS.WPFClient.Models
{
    public class SearchTypeModel : ObservableObject
    {
        private string? typeDes;
        /// <summary>
        /// 搜索类型描述
        /// </summary>
        public string? TypeDes
        {
            get { return typeDes; }
            set
            {
                this.SetProperty(ref typeDes, value);
            }
        }
    }
}
