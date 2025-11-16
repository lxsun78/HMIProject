using CommunityToolkit.Mvvm.Input;
using NPOI.SS.Formula.Functions;
using RS.Widgets.Controls;
using RS.Widgets.Interfaces;
using RS.Widgets.Models;
using RS.WPFClient.Enums;
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

        public ICommand SearchViewPreviewMouseLeftButtonUpCommand { get; }


        public ICommand SearchCommand { get; }

        public ICommand SearchCancelCommand { get; }

        #endregion
        public SearchViewModel()
        {
            this.HideSearchCommand = new RelayCommand(HideSearch);
            this.SearchViewPreviewMouseLeftButtonUpCommand = new RelayCommand(SearchViewPreviewMouseLeftButtonUp);
            this.SearchCommand = new RelayCommand(Search, CanSearch);
            this.SearchCancelCommand = new RelayCommand(SearchCancel);

            this.InitSearchTypeList();
            this.InitDateFilterTypeList();
            this.InitLocationTypeList();
            this.InitFolderTypeList();
            this.InitReadUnReadTypeList();
            this.InitAttachmentTypeList();


        }

        private void Search()
        {
            this.IsShowSearch = true;
        }

        private bool CanSearch()
        {
            return false;
        }

        private void SearchCancel()
        {
            this.IsAdvancedSearchChecked = false;
        }

        private void SearchViewPreviewMouseLeftButtonUp()
        {
            this.IsShowSearch = true;
        }

        private void HideSearch()
        {
            //用户点击搜索类型或者高级搜索时不关闭
            if (this.IsSearchTypeChecked || this.IsAdvancedSearchChecked)
            {
                return;
            }

            //搜索内容不为空 则不关闭
            if (!string.IsNullOrWhiteSpace(this.SearchContent))
            {
                return;
            }

            IsShowSearch = false;
        }



        private bool isShowSearch;
        /// <summary>
        /// 是否选择搜索
        /// </summary>
        public bool IsShowSearch
        {
            get { return isShowSearch; }
            set
            {
                this.SetProperty(ref isShowSearch, value);
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


        private bool isAdvancedSearchChecked;
        /// <summary>
        /// 高级搜索是否选中
        /// </summary>
        public bool IsAdvancedSearchChecked
        {
            get { return isAdvancedSearchChecked; }
            set
            {
                this.SetProperty(ref isAdvancedSearchChecked, value);
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




        #region 搜索类型
        /// <summary>
        /// 初始化搜索类型
        /// </summary>
        private void InitSearchTypeList()
        {
            var dataList = GetDataList<SearchType>();
            this.SearchTypeList = new ObservableCollection<SearchType>(dataList);
            this.SearchTypeSelect = this.SearchTypeList.FirstOrDefault(t => t == SearchType.Email);
        }


        private ObservableCollection<SearchType> searchTypeList;
        /// <summary>
        /// 搜索类型
        /// </summary>
        public ObservableCollection<SearchType> SearchTypeList
        {
            get { return searchTypeList; }
            set
            {
                this.SetProperty(ref searchTypeList, value);
            }
        }

        private SearchType searchTypeSelect;
        /// <summary>
        /// 搜索类型
        /// </summary>
        public SearchType SearchTypeSelect
        {
            get { return searchTypeSelect; }
            set
            {
                this.SetProperty(ref searchTypeSelect, value);
            }
        }
        #endregion


        #region 关键字位置
        /// <summary>
        /// 初始化关键位置
        /// </summary>
        private void InitLocationTypeList()
        {
            var dataList = GetDataList<LocationType>();
            this.LocationTypeList = new ObservableCollection<LocationType>(dataList);
            this.LocationType = this.LocationTypeList.FirstOrDefault(t => t == LocationType.Any);
        }

        private ObservableCollection<LocationType> locationTypeList;
        /// <summary>
        /// 关键字位置列表
        /// </summary>
        public ObservableCollection<LocationType> LocationTypeList
        {
            get
            {
                return locationTypeList;
            }
            set
            {
                this.SetProperty(ref locationTypeList, value);
            }
        }


        private LocationType locationType;
        /// <summary>
        /// 关键字位置类型
        /// </summary>
        public LocationType LocationType
        {
            get { return locationType; }
            set
            {
                this.SetProperty(ref locationType, value);
            }
        }
        #endregion


        #region 日期
        /// <summary>
        /// 初始化日期
        /// </summary>
        private void InitDateFilterTypeList()
        {
            var dataList = GetDataList<DateFilterType>();
            this.DateFilterTypeList = new ObservableCollection<DateFilterType>(dataList);
            this.DateFilterTypeSelect = this.DateFilterTypeList.FirstOrDefault(t => t == DateFilterType.Any);
        }

        private ObservableCollection<DateFilterType> dateFilterTypeList;
        /// <summary>
        /// 日期
        /// </summary>
        public ObservableCollection<DateFilterType> DateFilterTypeList
        {
            get
            {
                return dateFilterTypeList;
            }
            set
            {
                this.SetProperty(ref dateFilterTypeList, value);
            }
        }


        private DateFilterType dateFilterTypeSelect;
        /// <summary>
        /// 日期搜索类型
        /// </summary>
        public DateFilterType DateFilterTypeSelect
        {
            get { return dateFilterTypeSelect; }
            set
            {
                this.SetProperty(ref dateFilterTypeSelect, value);
            }
        }
        #endregion


       

        #region 所在文件夹

        /// <summary>
        /// 初始化所在文件
        /// </summary>
        private void InitFolderTypeList()
        {
            var dataList = GetDataList<FolderType>();
            this.FolderTypeList = new ObservableCollection<FolderType>(dataList);
            this.FolderTypeSelect = this.FolderTypeList.FirstOrDefault(t => t == FolderType.Any);
        }

        private ObservableCollection<FolderType> folderTypeList;
        /// <summary>
        /// 所在文件数据
        /// </summary>
        public ObservableCollection<FolderType> FolderTypeList
        {
            get
            {
                return folderTypeList;
            }
            set
            {
                this.SetProperty(ref folderTypeList, value);
            }
        }


        private FolderType folderTypeSelect;
        /// <summary>
        /// 所在文件选择
        /// </summary>
        public FolderType FolderTypeSelect
        {
            get { return folderTypeSelect; }
            set
            {
                this.SetProperty(ref folderTypeSelect, value);
            }
        }
        #endregion


        #region 已读/未读

        /// <summary>
        /// 初始化已读/未读
        /// </summary>
        private void InitReadUnReadTypeList()
        {
            var dataList = GetDataList<ReadUnReadType>();
            this.ReadUnReadTypeList = new ObservableCollection<ReadUnReadType>(dataList);
            this.ReadUnReadTypeSelect = this.ReadUnReadTypeList.FirstOrDefault(t => t == ReadUnReadType.Any);
        }

        private ObservableCollection<ReadUnReadType> readUnReadTypeList;
        /// <summary>
        /// 已读/未读 列表
        /// </summary>
        public ObservableCollection<ReadUnReadType> ReadUnReadTypeList
        {
            get
            {
                return readUnReadTypeList;
            }
            set
            {
                this.SetProperty(ref readUnReadTypeList, value);
            }
        }


        private ReadUnReadType readUnReadTypeSelect;
        /// <summary>
        /// 已读/未读 选择
        /// </summary>
        public ReadUnReadType ReadUnReadTypeSelect
        {
            get { return readUnReadTypeSelect; }
            set
            {
                this.SetProperty(ref readUnReadTypeSelect, value);
            }
        }
        #endregion


        #region 是否包含附件

        /// <summary>
        /// 初始化是否包含附件
        /// </summary>
        private void InitAttachmentTypeList()
        {
            var dataList = GetDataList<AttachmentType>();
            this.AttachmentTypeList = new ObservableCollection<AttachmentType>(dataList);
            this.AttachmentTypeSelect = this.AttachmentTypeList.FirstOrDefault(t => t == AttachmentType.Any);
        }

        private ObservableCollection<AttachmentType> attachmentTypeList;
        /// <summary>
        /// 是否包含附件
        /// </summary>
        public ObservableCollection<AttachmentType> AttachmentTypeList
        {
            get
            {
                return attachmentTypeList;
            }
            set
            {
                this.SetProperty(ref attachmentTypeList, value);
            }
        }


        private AttachmentType attachmentTypeSelect;
        /// <summary>
        /// 是否包含附件选择
        /// </summary>
        public AttachmentType AttachmentTypeSelect
        {
            get { return attachmentTypeSelect; }
            set
            {
                this.SetProperty(ref attachmentTypeSelect, value);
            }
        }
        #endregion





        private List<T> GetDataList<T>()
        {
            return Enum.GetValues(typeof(T))
               .Cast<T>().ToList();
        }
    }
}
