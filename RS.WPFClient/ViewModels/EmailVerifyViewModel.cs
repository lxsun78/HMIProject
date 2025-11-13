using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RS.Commons;
using RS.Commons.Attributs;
using RS.WPFClient.IServices;
using RS.WPFClient.Models;
using RS.WPFClient.IBLL;
using RS.Models;
using RS.Server.WebAPI;
using RS.Widgets.Enums;
using RS.Widgets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RS.WPFClient.ViewModels
{
    [ServiceInjectConfig(ServiceLifetime.Transient)]
    public class EmailVerifyViewModel : ViewModelBase
    {
        public EmailVerifyViewModel()
        {
            
        }
    }
}
