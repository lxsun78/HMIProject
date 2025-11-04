using RS.Commons;
using RS.Server.Models;
using RS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Server.IBLL
{
    public interface IOpenCVBLL
    {
        Task<OperateResult<ImgVerifyInitModel>> GetVerifyImgInitModelAsync();
    }
}
