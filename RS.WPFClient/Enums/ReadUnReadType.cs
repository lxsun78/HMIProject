using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.WPFClient.Enums
{

    public enum ReadUnReadType
    {
        /// <summary>
        /// 任何状态（不限制阅读状态，包含已读和未读的全部内容）
        /// </summary>
        Any = -1,

        /// <summary>
        /// 未读（内容尚未被查看或标记为已读）
        /// </summary>
        UnRead,

        /// <summary>
        /// 已读（内容已被查看或手动标记为已读状态）
        /// </summary>
        Read,
    }
}
