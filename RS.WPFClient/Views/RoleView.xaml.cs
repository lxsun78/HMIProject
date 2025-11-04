using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using RS.WPFClient.Client.Models;
using RS.Widgets.Controls;
using RS.Widgets.Models;
using System.Windows;

namespace RS.WPFClient.Client.Views
{
    /// <summary>
    /// 角色管理视图
    /// </summary>
    public partial class RoleView : RSDialog
    {
        public RoleView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<CRUDViewModel<RoleModel>>(this, HandleFormMessage);
        }

        private void HandleFormMessage(object recipient, CRUDViewModel<RoleModel> message)
        {
            var userFormView = App.ServiceProvider.GetRequiredService<UserFormView>();
            var rsForm = new RSForm(userFormView,message);
            rsForm.Owner = (Window)this.ParentWin;
            rsForm.Closed += RsForm_Closed;
            rsForm.Show();
        }

        private void RsForm_Closed(object? sender, EventArgs e)
        {
            var window = (Window)this.ParentWin;
            window?.Activate();
        }

    }
}
