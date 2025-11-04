using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using RS.Commons.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RS.Widgets
{
    public static class WidgetsServiceExtensions
    {
        public static IServiceCollection RegisterWidgetsService(this IServiceCollection services)
        {
            services.RegisterService(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
