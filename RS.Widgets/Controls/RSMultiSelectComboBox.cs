using CommunityToolkit.Mvvm.Input;
using RS.Widgets.Models;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace RS.Widgets.Controls
{
    public class RSMultiSelectComboBox : DataGrid
    {
        private ItemsControl? PART_TagContent;
        private TextBox? PART_SearchContent;
        private bool isSyncingSelectedItems = false; // 标志：正在同步选中项，避免触发SelectionChanged事件
        private bool isTagSelecting = false; // 标志：正在处理标签选择，防止ToggleDropDownCommand执行
        private ObservableCollection<TagModel>? currentTagModelList;

        static RSMultiSelectComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RSMultiSelectComboBox), new FrameworkPropertyMetadata(typeof(RSMultiSelectComboBox)));
        }

        public RSMultiSelectComboBox()
        {
            // 初始化 TagModelList
            TagModelList = new ObservableCollection<TagModel>();
            
            SetValue(TagCloseCommandPropertyKey, new RelayCommand<TagModel>(TagCloseExecute));
            SetValue(TagSelectCommandPropertyKey, new RelayCommand<TagModel>(TagSelectExecute));
            SetValue(ClearAllTagsCommandPropertyKey, new RelayCommand(ClearAllTagsExecute));
            SetValue(ToggleDropDownCommandPropertyKey, new RelayCommand(ToggleDropDownExecute));
            this.SelectionChanged += OnSelectionChanged;
            this.Columns.CollectionChanged += OnColumnsCollectionChanged;
            this.PreviewKeyDown += OnPreviewKeyDown;
        }

        private void OnColumnsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AdjustColumnWidthWhenSingleColumn();
        }

        /// <summary>
        /// 当只有1列时，自动设置宽度为*填满可用空间
        /// </summary>
        private void AdjustColumnWidthWhenSingleColumn()
        {
            if (this.Columns.Count == 1)
            {
                this.Columns[0].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果正在同步选中项，跳过处理
            if (isSyncingSelectedItems)
            {
                return;
            }

            // 下拉框只负责选择添加，将新增的选中项添加到TagModelList
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                // 如果是 Single 模式，先清除所有标签（只允许选择一个）
                if (this.SelectionMode == DataGridSelectionMode.Single)
                {
                    // 清除所有标签
                    TagModelList.Clear();
                    // 同步到 MultiSelectedItems
                    var multiSelectedItems = this.MultiSelectedItems as IList;
                    multiSelectedItems?.Clear();
                }

                foreach (var item in e.AddedItems)
                {
                    var tagModel = CreateTagModel(item, TagItemSource.FromItemsSource);
                    // 检查是否已存在，避免重复添加
                    if (!TagModelList.Contains(tagModel))
                    {
                        TagModelList.Add(tagModel);
                        // 同步到 MultiSelectedItems
                        var multiSelectedItems = this.MultiSelectedItems as IList;
                        if (multiSelectedItems != null)
                        {
                            multiSelectedItems.Add(tagModel.Data);
                        }
                    }
                }
                UpdateHasContent();
            }
        }

        public string TagDisplayMemberPath
        {
            get { return (string)GetValue(TagDisplayMemberPathProperty); }
            set { SetValue(TagDisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty TagDisplayMemberPathProperty =
            DependencyProperty.Register(nameof(TagDisplayMemberPath), typeof(string), typeof(RSMultiSelectComboBox), new PropertyMetadata(null));

        /// <summary>
        /// 这是实际选择的数据 实际选择数据肯定是从数据源来的
        /// </summary>
        public IEnumerable MultiSelectedItems
        {
            get { return (IEnumerable)GetValue(MultiSelectedItemsProperty); }
            set { SetValue(MultiSelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty MultiSelectedItemsProperty =
            DependencyProperty.Register(nameof(MultiSelectedItems), typeof(IEnumerable), typeof(RSMultiSelectComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 这是选择Tag数据 这个标签则可能和SelectedItems 个数不一样
        /// </summary>
        public ObservableCollection<TagModel> TagModelList
        {
            get { return (ObservableCollection<TagModel>)GetValue(TagModelListProperty); }
            set { SetValue(TagModelListProperty, value); }
        }

        public static readonly DependencyProperty TagModelListProperty =
            DependencyProperty.Register(nameof(TagModelList), typeof(ObservableCollection<TagModel>), typeof(RSMultiSelectComboBox), new FrameworkPropertyMetadata(new ObservableCollection<TagModel>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTagModelListChanged));

        private static void OnTagModelListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RSMultiSelectComboBox)d;
            control.OnTagModelListChanged((ObservableCollection<TagModel>?)e.OldValue, (ObservableCollection<TagModel>?)e.NewValue);
        }

        private void OnTagModelListChanged(ObservableCollection<TagModel>? oldValue, ObservableCollection<TagModel>? newValue)
        {
            // 取消订阅旧的集合的CollectionChanged事件
            if (oldValue != null && oldValue == currentTagModelList)
            {
                oldValue.CollectionChanged -= TagModelList_CollectionChanged;
                currentTagModelList = null;
            }

            // 订阅新的集合的CollectionChanged事件
            if (newValue != null)
            {
                newValue.CollectionChanged += TagModelList_CollectionChanged;
                currentTagModelList = newValue;
            }

            // 更新HasContent属性
            UpdateHasContent();
        }

        private void TagModelList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // 当TagModelList集合发生变化时，更新HasContent属性
            UpdateHasContent();
        }

        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(RSMultiSelectComboBox), new PropertyMetadata(300D));

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(RSMultiSelectComboBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged));

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RSMultiSelectComboBox)d;
            if ((bool)e.NewValue == true)
            {
                // 当下拉框打开时，根据TagModelList同步SelectedItems
                control.SyncSelectedItemsFromTagModelList();
            }
            else
            {
                // 当下拉框关闭时，清除筛选
                control.ClearItemsSourceFilter();
                // 清空搜索内容
                control.SearchContent = string.Empty;
            }
        }



        public string SearchContent
        {
            get { return (string)GetValue(SearchContentProperty); }
            set { SetValue(SearchContentProperty, value); }
        }

        public static readonly DependencyProperty SearchContentProperty =
            DependencyProperty.Register(nameof(SearchContent), typeof(string), typeof(RSMultiSelectComboBox), new PropertyMetadata(null, OnSearchContentChanged));

        private static void OnSearchContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RSMultiSelectComboBox)d;
            control.OnSearchContentChanged((string?)e.OldValue, (string?)e.NewValue);
        }

        private void OnSearchContentChanged(string? oldValue, string? newValue)
        {
            // 当搜索内容变化时，筛选数据源
            FilterItemsSource();
        }

        public bool ShowSearchBox
        {
            get { return (bool)GetValue(ShowSearchBoxProperty); }
            set { SetValue(ShowSearchBoxProperty, value); }
        }

        public static readonly DependencyProperty ShowSearchBoxProperty =
            DependencyProperty.Register(nameof(ShowSearchBox), typeof(bool), typeof(RSMultiSelectComboBox), new PropertyMetadata(true));



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_TagContent = this.GetTemplateChild("PART_TagContent") as ItemsControl;
            this.PART_SearchContent = this.GetTemplateChild("PART_SearchContent") as TextBox;

            if (this.PART_SearchContent != null)
            {
                this.PART_SearchContent.KeyDown += OnSearchContentKeyDown;
            }
        }

        private void OnSearchContentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleSearchEnterKey();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 处理搜索框回车键：添加第一个未添加的项，或使用输入内容创建标签
        /// </summary>
        private void HandleSearchEnterKey()
        {
            if (string.IsNullOrWhiteSpace(this.SearchContent))
            {
                return;
            }

            // 如果绑定了 TagDisplayMemberPath，尝试从筛选结果中找到第一个未添加的项
            if (!string.IsNullOrEmpty(this.TagDisplayMemberPath) && this.ItemsSource != null)
            {
                var searchText = this.SearchContent.Trim();
                var filteredItems = GetFilteredItems(searchText);
                
                // 找到第一个未添加的项
                foreach (var item in filteredItems)
                {
                    var tagModel = CreateTagModel(item, TagItemSource.FromItemsSource);
                    if (!TagModelList.Contains(tagModel))
                    {
                        // 如果是 Single 模式，先清除所有标签（只允许选择一个）
                        if (this.SelectionMode == DataGridSelectionMode.Single)
                        {
                            // 清除所有标签
                            TagModelList.Clear();
                            // 同步到 MultiSelectedItems
                            var multiSelectedItems = this.MultiSelectedItems as IList;
                            multiSelectedItems?.Clear();
                        }

                        // 添加到 TagModelList
                        TagModelList.Add(tagModel);
                        // 同步到 MultiSelectedItems
                        var multiSelectedItems2 = this.MultiSelectedItems as IList;
                        if (multiSelectedItems2 != null)
                        {
                            multiSelectedItems2.Add(tagModel.Data);
                        }
                        UpdateHasContent();
                        
                        // 清空搜索内容
                        this.SearchContent = string.Empty;
                        return;
                    }
                }
            }

            // 如果没有找到匹配的项或没有绑定 TagDisplayMemberPath，使用输入内容创建标签（Custom类型）
            object tagData = this.SearchContent.Trim();
            var customTagModel = CreateTagModel(tagData, TagItemSource.Custom);
            // 检查是否已存在
            if (!TagModelList.Contains(customTagModel))
            {
                // 如果是 Single 模式，先清除所有标签（只允许选择一个）
                if (this.SelectionMode == DataGridSelectionMode.Single)
                {
                    // 清除所有标签
                    TagModelList.Clear();
                    // 同步到 MultiSelectedItems
                    var multiSelectedItems = this.MultiSelectedItems as IList;
                    multiSelectedItems?.Clear();
                }

                TagModelList.Add(customTagModel);
                UpdateHasContent();
            }

            // 清空搜索内容
            this.SearchContent = string.Empty;
        }

        /// <summary>
        /// 根据搜索文本筛选数据源
        /// </summary>
        private void FilterItemsSource()
        {
            if (string.IsNullOrEmpty(this.TagDisplayMemberPath) || this.ItemsSource == null)
            {
                // 如果没有绑定 TagDisplayMemberPath，不进行筛选，清除筛选
                ClearItemsSourceFilter();
                return;
            }

            var searchText = this.SearchContent?.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                // 如果搜索文本为空，显示所有数据
                ClearItemsSourceFilter();
                return;
            }

            // 使用 CollectionView 进行筛选
            var collectionView = CollectionViewSource.GetDefaultView(this.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = item =>
                {
                    var displayValue = ReflectionHelper.GetDisplayMemberValue(item, this.TagDisplayMemberPath);
                    var displayText = displayValue?.ToString() ?? item?.ToString() ?? string.Empty;
                    return displayText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                };
            }
        }

        /// <summary>
        /// 清除数据源筛选
        /// </summary>
        private void ClearItemsSourceFilter()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            var collectionView = CollectionViewSource.GetDefaultView(this.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = null;
            }
        }

        /// <summary>
        /// 获取筛选后的数据项列表
        /// </summary>
        private IEnumerable<object> GetFilteredItems(string searchText)
        {
            if (this.ItemsSource == null || string.IsNullOrEmpty(searchText))
            {
                return Enumerable.Empty<object>();
            }

            var results = new List<object>();
            foreach (var item in this.ItemsSource)
            {
                var displayValue = ReflectionHelper.GetDisplayMemberValue(item, this.TagDisplayMemberPath);
                var displayText = displayValue?.ToString() ?? item?.ToString() ?? string.Empty;
                
                if (displayText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public bool HasContent
        {
            get { return (bool)GetValue(HasContentProperty); }
            set { SetValue(HasContentProperty, value); }
        }

        public static readonly DependencyProperty HasContentProperty =
            DependencyProperty.Register(nameof(HasContent), typeof(bool), typeof(RSMultiSelectComboBox), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey TagCloseCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "TagCloseCommand",
                typeof(ICommand),
                typeof(RSMultiSelectComboBox),
                new PropertyMetadata(null));
        public static readonly DependencyProperty TagCloseCommandProperty = TagCloseCommandPropertyKey.DependencyProperty;

        public ICommand TagCloseCommand => (ICommand)GetValue(TagCloseCommandProperty);

        private static readonly DependencyPropertyKey TagSelectCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "TagSelectCommand",
                typeof(ICommand),
                typeof(RSMultiSelectComboBox),
                new PropertyMetadata(null));
        public static readonly DependencyProperty TagSelectCommandProperty = TagSelectCommandPropertyKey.DependencyProperty;

        public ICommand TagSelectCommand => (ICommand)GetValue(TagSelectCommandProperty);

        private static readonly DependencyPropertyKey ClearAllTagsCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ClearAllTagsCommand",
                typeof(ICommand),
                typeof(RSMultiSelectComboBox),
                new PropertyMetadata(null));
        public static readonly DependencyProperty ClearAllTagsCommandProperty = ClearAllTagsCommandPropertyKey.DependencyProperty;

        public ICommand ClearAllTagsCommand => (ICommand)GetValue(ClearAllTagsCommandProperty);

        private static readonly DependencyPropertyKey ToggleDropDownCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ToggleDropDownCommand",
                typeof(ICommand),
                typeof(RSMultiSelectComboBox),
                new PropertyMetadata(null));
        public static readonly DependencyProperty ToggleDropDownCommandProperty = ToggleDropDownCommandPropertyKey.DependencyProperty;

        public ICommand ToggleDropDownCommand => (ICommand)GetValue(ToggleDropDownCommandProperty);

        private int selectedTagIndex = -1;
        public int SelectedTagIndex
        {
            get { return selectedTagIndex; }
            private set
            {
                if (selectedTagIndex != value)
                {
                    selectedTagIndex = value;
                    UpdateTagSelectionVisual();
                }
            }
        }

        /// <summary>
        /// 创建 TagModel 实例
        /// </summary>
        private TagModel CreateTagModel(object data, TagItemSource source)
        {
            object? tagContent;
            if (source == TagItemSource.FromItemsSource)
            {
                // 来自 ItemsSource 的数据，使用 TagDisplayMemberPath 获取显示内容
                tagContent = ReflectionHelper.GetDisplayMemberValue(data, this.TagDisplayMemberPath) ?? data;
            }
            else
            {
                // 用户自定义输入的数据，直接使用数据本身作为显示内容
                tagContent = data?.ToString() ?? data;
            }

            return new TagModel()
            {
                TagContent = tagContent,
                IsSelect = false,
                Data = data,
                Source = source
            };
        }

        private void UpdateHasContent()
        {
            this.HasContent = this.TagModelList.Count > 0;
        }

        private void TagSelectExecute(TagModel? tag)
        {
            if (tag == null)
            {
                return;
            }

            // 设置标志，防止后续的 ToggleDropDownCommand 执行
            this.isTagSelecting = true;

            try
            {
                int index = this.TagModelList.IndexOf(tag);
                if (index >= 0)
                {
                    // 如果点击的是已经选中的 Tag，则切换下拉框状态
                    if (this.SelectedTagIndex == index)
                    {
                        // 切换下拉框的打开/关闭状态
                        this.IsDropDownOpen = !this.IsDropDownOpen;
                    }
                    else
                    {
                        // 点击未选中的 Tag，选中它并关闭下拉框
                        SelectTag(index);
                        this.IsDropDownOpen = false;
                    }
                    this.Focus();
                }
            }
            finally
            {
                // 使用 Dispatcher.BeginInvoke 延迟重置标志，确保 ToggleDropDownCommand 检查时能看到这个标志
                this.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    this.isTagSelecting = false;
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private void TagCloseExecute(TagModel? tag)
        {
            if (tag == null)
            {
                return;
            }

            int index = this.TagModelList.IndexOf(tag);
            if (index < 0)
            {
                return;
            }

            // 保存当前选中索引（删除前的）
            int currentSelectedIndex = this.SelectedTagIndex;

            // 从 TagModelList 中移除
            TagModelList.RemoveAt(index);

            // 只从 SelectedItems 中移除来自 ItemsSource 的项
            if (tag.Source == TagItemSource.FromItemsSource && tag.Data != null)
            {
                this.SelectedItems.Remove(tag.Data);
                
                // 同步到 MultiSelectedItems
                var multiSelectedItems = this.MultiSelectedItems as IList;
                multiSelectedItems?.Remove(tag.Data);
            }
            
            UpdateHasContent();

            // 调整选中索引
            if (this.TagModelList.Count == 0)
            {
                SelectedTagIndex = -1;
            }
            else
            {
                // 如果删除的是当前选中的标签，清除选中状态（不自动选中其他标签）
                if (currentSelectedIndex == index)
                {
                    // 删除的是当前选中的标签，清除选中状态
                    SelectedTagIndex = -1;
                }
                else if (currentSelectedIndex > index)
                {
                    // 删除的标签在当前选中标签之前，需要将选中索引减1
                    SelectTag(currentSelectedIndex - 1);
                }
                // 如果 currentSelectedIndex < index，删除的标签在当前选中标签之后，不需要调整
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.TagModelList.Count == 0)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    if (this.SelectedTagIndex > 0)
                    {
                        SelectTag(this.SelectedTagIndex - 1);
                        e.Handled = true;
                    }
                    else if (this.SelectedTagIndex == -1)
                    {
                        // 如果没有选中，选中最后一个
                        SelectTag(this.TagModelList.Count - 1);
                        e.Handled = true;
                    }
                    break;

                case Key.Right:
                    if (this.SelectedTagIndex >= 0 && this.SelectedTagIndex < this.TagModelList.Count - 1)
                    {
                        SelectTag(this.SelectedTagIndex + 1);
                        e.Handled = true;
                    }
                    else if (this.SelectedTagIndex < 0 && this.TagModelList.Count > 0)
                    {
                        // 如果没有选中，选中第一个
                        SelectTag(0);
                        e.Handled = true;
                    }
                    // 已经选中最后一个，保持选中状态不变
                    break;

                case Key.Back:
                    if (this.SelectedTagIndex >= 0)
                    {
                        DeleteSelectedTag();
                        e.Handled = true;
                    }
                    else if (this.TagModelList.Count > 0)
                    {
                        // 没有选中时，删除最后一个Tag
                        var lastTag = this.TagModelList[this.TagModelList.Count - 1];
                        TagCloseExecute(lastTag);
                        e.Handled = true;
                    }
                    break;

                case Key.Delete:
                    if (this.SelectedTagIndex >= 0)
                    {
                        DeleteSelectedTag();
                        e.Handled = true;
                    }
                    break;

                case Key.Escape:
                    if (this.SelectedTagIndex >= 0)
                    {
                        ClearTagSelect();
                        e.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// 选择指定索引的Tag
        /// </summary>
        private void SelectTag(int index)
        {
            if (index < 0 || this.TagModelList.Count == 0)
            {
                this.SelectedTagIndex = -1;
                return;
            }

            if (index >= this.TagModelList.Count)
            {
                index = this.TagModelList.Count - 1;
            }

            this.SelectedTagIndex = index;

            // 使选中的Tag滚动到可见位置
            ScrollTagIntoView(index);
        }

        /// <summary>
        /// 将指定索引的Tag滚动到可见位置
        /// </summary>
        private void ScrollTagIntoView(int index)
        {
            if (this.PART_TagContent == null || index < 0 || index >= this.TagModelList.Count)
            {
                return;
            }

            // 使用 Dispatcher 确保 UI 已经更新
            this.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                try
                {
                    // 通过 ItemContainerGenerator 获取容器
                    var generator = this.PART_TagContent.ItemContainerGenerator;
                    if (generator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                    {
                        var container = generator.ContainerFromIndex(index) as FrameworkElement;
                        if (container != null)
                        {
                            // 查找 Border 元素（Tag 的根元素）
                            var border = container.FindChild<Border>("PART_TagItemBorder");
                            if (border != null)
                            {
                                border.BringIntoView();
                            }
                            else
                            {
                                // 如果找不到 Border，直接让容器滚动到可见位置
                                container.BringIntoView();
                            }
                        }
                    }
                }
                catch
                {
                    // 忽略错误，避免影响正常功能
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }



        /// <summary>
        /// 根据SelectedTagIndex更新所有Tag的选中状态视觉效果
        /// </summary>
        private void UpdateTagSelectionVisual()
        {
            if (this.TagModelList == null)
            {
                return;
            }

            for (int i = 0; i < this.TagModelList.Count; i++)
            {
                this.TagModelList[i].IsSelect = (i == this.SelectedTagIndex);
            }
        }

        /// <summary>
        /// 清除Tag的选中状态
        /// </summary>
        private void ClearTagSelect()
        {
            this.SelectedTagIndex = -1;
        }

        /// <summary>
        /// 删除当前选中的Tag
        /// </summary>
        private void DeleteSelectedTag()
        {
            if (this.SelectedTagIndex < 0 || this.SelectedTagIndex >= this.TagModelList.Count)
            {
                return;
            }

            var tagToDelete = this.TagModelList[this.SelectedTagIndex];
            TagCloseExecute(tagToDelete);
        }

        /// <summary>
        /// 切换下拉框的打开/关闭状态（用于空白区域点击）
        /// </summary>
        private void ToggleDropDownExecute()
        {
            // 如果正在处理标签选择，跳过执行，避免事件冒泡导致的重复切换
            if (this.isTagSelecting)
            {
                return;
            }

            this.IsDropDownOpen = !this.IsDropDownOpen;
        }

        /// <summary>
        /// 清除所有Tag标签
        /// </summary>
        private void ClearAllTagsExecute()
        {
            if (this.TagModelList.Count == 0)
            {
                return;
            }

            // 清除所有Tag
            TagModelList.Clear();
            this.SelectedItems.Clear();
            
            // 同步到 MultiSelectedItems
            var multiSelectedItems = this.MultiSelectedItems as IList;
            multiSelectedItems?.Clear();

            UpdateHasContent();

            // 清除选中索引
            SelectedTagIndex = -1;
        }

        /// <summary>
        /// 根据TagModelList同步DataGrid的SelectedItems，使下拉框打开时显示已选择的项
        /// </summary>
        private void SyncSelectedItemsFromTagModelList()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            try
            {
                isSyncingSelectedItems = true;

                // 先清除当前选中项
                this.SelectedItems.Clear();

                // 如果 TagModelList 为空，直接返回（SelectedItems 已清除）
                if (TagModelList.Count == 0)
                {
                    return;
                }

                // 获取实际的视图（考虑筛选）
                var collectionView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                var itemsToIterate = collectionView?.Cast<object>() ?? this.ItemsSource.Cast<object>();

                // 遍历ItemsSource，如果项在TagModelList中且来源是FromItemsSource，则添加到SelectedItems
                foreach (var item in itemsToIterate)
                {
                    var tagModel = CreateTagModel(item, TagItemSource.FromItemsSource);
                    if (TagModelList.Contains(tagModel))
                    {
                        this.SelectedItems.Add(item);
                        
                        // 如果是 Single 模式，只添加第一个匹配的项，然后退出
                        if (this.SelectionMode == DataGridSelectionMode.Single)
                        {
                            break;
                        }
                    }
                }
            }
            finally
            {
                isSyncingSelectedItems = false;
            }
        }
    }
}

