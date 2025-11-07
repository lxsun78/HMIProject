using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RS.Commons;
using RS.Commons.Attributs;
using RS.Commons.Extensions;
using RS.WPFClient.Client.Models;
using RS.Server.WebAPI;
using RS.Widgets.Interfaces;
using RS.Widgets.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace RS.WPFClient.Client.ViewModels
{

    /// <summary>
    /// 用户管理视图模型
    /// </summary>
    [ServiceInjectConfig(ServiceLifetime.Transient)]
    public class UserViewModel : CRUDViewModel<UserModel>, INavigate
    {

        /// <summary>
        /// 用户启用
        /// </summary>
        public ICommand UserEnableCommand { get; set; }

        /// <summary>
        /// 用户禁用
        /// </summary>
        public ICommand UserDisableCommand { get; set; }

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public UserViewModel()
        {
            this.UserEnableCommand = new RelayCommand<UserModel>(UserEnable);
            this.UserDisableCommand = new RelayCommand<UserModel>(UserDisable);
        }



        #region 依赖属性

        private ObservableCollection<ComboBoxItemModel<bool>> isDisableSelectList;
        /// <summary>
        /// 用户是否禁用选择
        /// </summary>
        public ObservableCollection<ComboBoxItemModel<bool>> IsDisableSelectList
        {
            get
            {
                if (isDisableSelectList == null)
                {
                    isDisableSelectList = new ObservableCollection<ComboBoxItemModel<bool>>();
                    isDisableSelectList.Add(new ComboBoxItemModel<bool>()
                    {
                        Key = true,
                        KeyDes = "已禁用",
                    });

                    isDisableSelectList.Add(new ComboBoxItemModel<bool>()
                    {
                        Key = false,
                        KeyDes = "未禁用",
                    });
                }
                return isDisableSelectList;
            }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 查询命令
        /// </summary>
        public override async void SearchClick()
        {
            
            LoadingConfig loadingConfig = new LoadingConfig();
            var operateResult = await this.Navigate.Loading.InvokeAsync(async (cancellationToken) =>
            {
                await Task.Delay(2000);
                return OperateResult.CreateSuccessResult();
            }, loadingConfig: loadingConfig);

            if (!operateResult.IsSuccess)
            {
                await this.MessageBox.ShowMessageAsync(operateResult.Message, "错误提示");
            }
        }

        /// <summary>
        /// 查询条件清除命令
        /// </summary>
        public async Task SearchClearClick(object obj)
        {
            this.ModelSearch = new UserModel();
        }

        /// <summary>
        /// 关闭命令
        /// </summary>
        public void CloseClick(object obj)
        {

        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public override void DeleteClick(UserModel userModel)
        {
            //if (this.ModelSelect != null)
            //{
            //    this.ModelList.Remove(this.ModelSelect);
            //}
        }



        /// <summary>
        /// 查看数据
        /// </summary>
        public override void DetailsClick(UserModel userModel)
        {

        }

        /// <summary>
        /// 导出数据
        /// </summary>
        public override void ExportClick(ObservableCollection<UserModel>? collection)
        {

        }

        public override async Task PaginationAsync(Pagination pagination)
        {
            LoadingConfig loadingConfig = new LoadingConfig();
            var operateResult = await this.Navigate.Loading.InvokeAsync(async (cancellationToken) =>
               {
                   await Task.Delay(5000);
                   var dataResult = await HMIWebAPI.User.GetUser.AESHttpPostAsync<Pagination, RS.Models.PageDataModel<UserModel>>(pagination, nameof(HMIWebAPI));
                   if (!dataResult.IsSuccess)
                   {
                       return dataResult;
                   }
                   var pageDataModel = dataResult.Data;
                   pagination.Records = pageDataModel.Pagination.records;

                   var pageList = pageDataModel.DataList;

                   //回到UI线程更新集合
                   Application.Current.Dispatcher.Invoke(() =>
                   {
                       this.ModelList = new ObservableCollection<UserModel>(pageList);
                   });

                   return OperateResult.CreateSuccessResult();
               }, loadingConfig: loadingConfig);

            if (!operateResult.IsSuccess)
            {
                await this.MessageBox.ShowMessageAsync(operateResult.Message, "错误提示");
            }
        }




        public async void UserEnable(UserModel userModel)
        {
            await Task.Delay(3000);
        }


      


        public async void UserDisable(UserModel userModel)
        {
            await Task.Delay(3000);
        }

        public override async Task OnFormSubmitAsync(UserModel modelEidt)
        {

        }

        #endregion

    }
}
