using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.WPFClient.Enums
{

    public enum SearchType
    {
        /// <summary>
        /// 邮件（搜索对象为电子邮件相关内容）
        /// </summary>
        Email,

        /// <summary>
        /// 文件（搜索对象为各类存储的文件，如文档、图片等）
        /// </summary>
        File,

        /// <summary>
        /// 发票（搜索对象为发票类单据或相关记录）
        /// </summary>
        Invoice,

        /// <summary>
        /// 联系人（搜索对象为联系人信息，如姓名、联系方式等）
        /// </summary>
        Contacts,

        /// <summary>
        /// 笔记（搜索对象为笔记类内容，如个人记录、备注等）
        /// </summary>
        Notes,
    }
}
