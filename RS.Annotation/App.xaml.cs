using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RS.Annotation.Views;
using RS.Annotation.Views.Home;
using RS.Commons;
using RS.Commons.Extensions;
using RS.Models;
using RS.Server.WebAPI;
using RS.Widgets.Controls;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using RS.Widgets;
using RS.Widgets.Interfaces;

namespace RS.Annotation
{
    public partial class App : Application
    {
        public static HostApplicationBuilder ApplicationBuilder;

        public static IHost ApplicationHost;

        public static IServiceProvider ServiceProvider { get; set; }


        /// <summary>
        /// 日志服务
        /// </summary>
        public ILogBLL LogBLL { get; private set; }

        /// <summary>
        /// 程序入口
        /// </summary>
        public App()
        {
            //配置依赖注入服务
            ApplicationBuilder = Host.CreateApplicationBuilder();

            //注入缓存服务
            ApplicationBuilder.Services.AddMemoryCache();

            //注册控件库的服务
            ApplicationBuilder.Services.RegisterWidgetsService();

            //注册通用服务
            ApplicationBuilder.Services.RegisterCommonService();

            //注册日志服务
            ApplicationBuilder.Services.RegisterLog4netService();

            //注册方法拦截服务
            ApplicationBuilder.Services.RegisterInterceptorService();

            //注册当前程序集服务
            ApplicationBuilder.Services.RegisterService(Assembly.GetExecutingAssembly());
        

            //必须调用Build方法
            ApplicationHost = ApplicationBuilder.Build();

            ServiceProvider = ApplicationHost.Services;

            //开始异步执行
            ApplicationHost.RunAsync();


            //配置全局服务这样在其他程序集也可以访问服务
            ServiceProviderExtensions.ConfigServices(ServiceProvider);

            // 获取日志服务
            LogBLL = ServiceProvider.GetRequiredService<ILogBLL>();

            //程序未处理异常
            this.RegisterUnknowExceptionsHandler();
        }

       
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var loginView = App.ServiceProvider.GetRequiredService<LoginView>();
            loginView.Show();
            //var homeView = App.ServiceProvider?.GetRequiredService<HomeView>();
            //homeView.Show();
        }

     


        private void RegisterUnknowExceptionsHandler()
        {
#if RELEASE
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#endif
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogBLL.LogCritical(e.Exception, "App_DispatcherUnhandledException");
            e.Handled = true;
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogBLL.LogCritical(ex, "CurrentDomain_UnhandledException");
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogBLL.LogCritical(e.Exception, "TaskScheduler_UnobservedTaskException");
        }


    }
}
