using CommunityToolkit.Mvvm.Input;
using RS.Widgets.Interfaces;
using RS.Widgets.Models;
using RS.WPFClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RS.WPFClient.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        #region 依赖服务
        private readonly IWindowService WindowService;
        #endregion

        #region 命令
 
        public ICommand HideSearchCommand { get; }

        public ICommand TxtSearchGotFocusCommand { get; }
        #endregion
        public SearchViewModel()
        {
            this.HideSearchCommand = new RelayCommand(HideSearch);
            this.TxtSearchGotFocusCommand = new RelayCommand(TxtSearchGotFocus);
        }

        private void TxtSearchGotFocus()
        {
            this.IsSearchChecked = true;
        }

        private void HideSearch()
        {
            if (IsSearchTypeChecked)
            {
                return;
            }
            IsSearchChecked = false;
        }

      

        private bool isSearchChecked;
        /// <summary>
        /// 是否选择搜索
        /// </summary>
        public bool IsSearchChecked
        {
            get { return isSearchChecked; }
            set
            {
                this.SetProperty(ref isSearchChecked, value);
            }
        }


        private bool isSearchTypeChecked;
        /// <summary>
        /// 搜索类型是否选中
        /// </summary>
        public bool IsSearchTypeChecked
        {
            get { return isSearchTypeChecked; }
            set
            {
                this.SetProperty(ref isSearchTypeChecked, value);
            }
        }


        private string searchContent;
        /// <summary>
        /// 搜索内容
        /// </summary>
        public string SearchContent
        {
            get { return searchContent; }
            set
            {
                this.SetProperty(ref searchContent, value);
            }
        }




        private ObservableCollection<SearchTypeModel> searchTypeModelList;
        /// <summary>
        /// 搜索类型
        /// </summary>
        public ObservableCollection<SearchTypeModel> SearchTypeModelList
        {
            get { return searchTypeModelList; }
            set
            {
                this.SetProperty(ref searchTypeModelList, value);
            }
        }

    }
}
