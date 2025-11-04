using Microsoft.Extensions.DependencyInjection;
using RS.Commons.Attributs;
using RS.Server.DAL.SqlServer;
using RS.Server.IDAL;

namespace RS.Server.DAL
{

    /// <summary>
    /// 用户数据逻辑层
    /// </summary>
    [ServiceInjectConfig(typeof(ILoginDAL), ServiceLifetime.Transient)]
    internal class LoginDAL : Repository, ILoginDAL
    {
        public LoginDAL(RSAppDbContext rsAppDb)
        {
            this.RSAppDb = rsAppDb;
        }
    }
}
