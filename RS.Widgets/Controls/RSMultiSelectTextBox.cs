using CommunityToolkit.Mvvm.Input;
using RS.Widgets.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RS.Widgets.Controls
{
    public class RSMultiSelectTextBox : TextBox
    {

        private ScrollViewer PART_ContentHost;
        private ItemsControl PART_TagHost;
        private Button PART_BtnClear;

        public event Action<List<TagModel>> OnTagModelDeleteCallBack;
        public event Action<object, RoutedEventArgs> OnSelectTextBoxLostFocus;
        static RSMultiSelectTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RSMultiSelectTextBox), new FrameworkPropertyMetadata(typeof(RSMultiSelectTextBox)));
        }

        public RSMultiSelectTextBox()
        {
            this.TagModelList = new ObservableCollection<TagModel>();
            SetValue(TagCloseCommandPropertyKey, new RelayCommand<TagModel>(TagCloseExcute));
            SetValue(TagSelectCommandPropertyKey, new RelayCommand<TagModel>(TagSelectExcute));
            this.Loaded += RSMultiSelectTextBox_Loaded;
            this.TextChanged += RSMultiSelectTextBox_TextChanged;
            this.SizeChanged += RSMultiSelectTextBox_SizeChanged;
            this.PreviewKeyDown += RSMultiSelectTextBox_PreviewKeyDown;
            this.LostFocus += RSMultiSelectTextBox_LostFocus;
            this.GotFocus += RSMultiSelectTextBox_GotFocus;
        }

        private void RSMultiSelectTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateTagHostMargin();
        }

        private void RSMultiSelectTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("RSMultiSelectTextBox_LostFocus");
            SetTagHostMarginZero();
        }

        internal void RemoveMultiSelectTextBox_LostFocusEvent()
        {
            this.LostFocus -= RSMultiSelectTextBox_LostFocus;
        }

        internal void SetTagHostMarginZero()
        {
            this.PART_TagHost.Margin = new Thickness(0, 0, 0, 0);
        }


        private void TagSelectExcute(TagModel? model)
        {
            if (model == null)
            {
                return;
            }
            
            // 找到点击的Tag的索引
            int index = this.TagModelList.IndexOf(model);
            if (index >= 0)
            {
                SelectTag(index);
            }
        }

        private static readonly DependencyPropertyKey TagCloseCommandPropertyKey =
       DependencyProperty.RegisterReadOnly(
           "TagCloseCommand",
           typeof(ICommand),
           typeof(RSMultiSelectTextBox),
           new PropertyMetadata(null));
        public static readonly DependencyProperty TagCloseCommandProperty = TagCloseCommandPropertyKey.DependencyProperty;

        public ICommand TagCloseCommand => (ICommand)GetValue(TagCloseCommandProperty);


        private static readonly DependencyPropertyKey TagSelectCommandPropertyKey =
     DependencyProperty.RegisterReadOnly(
         "TagSelectCommand",
         typeof(ICommand),
         typeof(RSMultiSelectTextBox),
         new PropertyMetadata(null));
        public static readonly DependencyProperty TagSelectCommandProperty = TagSelectCommandPropertyKey.DependencyProperty;

        public ICommand TagSelectCommand => (ICommand)GetValue(TagSelectCommandProperty);


        public bool IsShowBtnClear
        {
            get { return (bool)GetValue(IsShowBtnClearProperty); }
            set { SetValue(IsShowBtnClearProperty, value); }
        }
        public static readonly DependencyProperty IsShowBtnClearProperty =
            DependencyProperty.Register(nameof(IsShowBtnClear), typeof(bool), typeof(RSMultiSelectTextBox), new PropertyMetadata(false));

        /// <summary>
        /// 当前选中的Tag索引，-1表示没有Tag被选中（焦点在文本输入区）
        /// </summary>
        public int SelectedTagIndex
        {
            get { return (int)GetValue(SelectedTagIndexProperty); }
            set { SetValue(SelectedTagIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedTagIndexProperty =
            DependencyProperty.Register(nameof(SelectedTagIndex), typeof(int), typeof(RSMultiSelectTextBox), new PropertyMetadata(-1));


        private void RSMultiSelectTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    // 当光标在输入框最左端或已有Tag被选中时，处理Tag导航
                    if (this.CaretIndex == 0 || this.SelectedTagIndex >= 0)
                    {
                        if (this.TagModelList.Count == 0)
                        {
                            return;
                        }

                        if (this.SelectedTagIndex < 0)
                        {
                            // 没有Tag被选中，选中最后一个Tag
                            SelectTag(this.TagModelList.Count - 1);
                        }
                        else if (this.SelectedTagIndex > 0)
                        {
                            // 选中前一个Tag
                            SelectTag(this.SelectedTagIndex - 1);
                        }
                        // 如果已经是第一个Tag，不做任何操作
                        e.Handled = true;
                    }
                    break;

                case Key.Right:
                    // 只有当有Tag被选中时才处理
                    if (this.SelectedTagIndex >= 0)
                    {
                        if (this.SelectedTagIndex < this.TagModelList.Count - 1)
                        {
                            // 选中下一个Tag
                            SelectTag(this.SelectedTagIndex + 1);
                        }
                        else
                        {
                            // 当前是最后一个Tag，取消选中并返回输入框
                            ClearTagSelect();
                            this.CaretIndex = 0;
                        }
                        e.Handled = true;
                    }
                    break;

                case Key.Back:
                    if (this.SelectedTagIndex >= 0)
                    {
                        // 有Tag被选中，删除它
                        DeleteSelectedTag();
                        e.Handled = true;
                    }
                    else if (this.Text.Length == 0 && this.TagModelList.Count > 0)
                    {
                        // 输入框为空，删除最后一个Tag
                        var lastTag = this.TagModelList[this.TagModelList.Count - 1];
                        this.TagModelList.RemoveAt(this.TagModelList.Count - 1);
                        this.UpdateHasContent();
                        OnTagModelDeleteCallBack?.Invoke(new List<TagModel>() { lastTag });
                        e.Handled = true;
                    }
                    break;

                case Key.Delete:
                    if (this.SelectedTagIndex >= 0)
                    {
                        // 有Tag被选中，删除它
                        DeleteSelectedTag();
                        e.Handled = true;
                    }
                    break;

                case Key.Escape:
                    // 取消所有Tag的选中状态
                    if (this.SelectedTagIndex >= 0)
                    {
                        ClearTagSelect();
                        e.Handled = true;
                    }
                    break;
            }
        }



        private void TagCloseExcute(TagModel? tagModel)
        {
            if (tagModel == null)
            {
                return;
            }
            this.TagModelList.Remove(tagModel);
            this.UpdateHasContent();
            OnTagModelDeleteCallBack?.Invoke(new List<TagModel>() { tagModel });
        }

        private void RSMultiSelectTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateTagHostMargin();
        }

        private void RSMultiSelectTextBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public bool HasContent
        {
            get { return (bool)GetValue(HasContentProperty); }
            set { SetValue(HasContentProperty, value); }
        }

        public static readonly DependencyProperty HasContentProperty =
            DependencyProperty.Register(nameof(HasContent), typeof(bool), typeof(RSMultiSelectTextBox), new PropertyMetadata(false));



        public ObservableCollection<TagModel> TagModelList
        {
            get { return (ObservableCollection<TagModel>)GetValue(TagModelListProperty); }
            set { SetValue(TagModelListProperty, value); }
        }

        public static readonly DependencyProperty TagModelListProperty =
            DependencyProperty.Register(nameof(TagModelList), typeof(ObservableCollection<TagModel>), typeof(RSMultiSelectTextBox), new FrameworkPropertyMetadata(new ObservableCollection<TagModel>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTagModelListPropertyChanged));

        private static void OnTagModelListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiSelectTextBox = d as RSMultiSelectTextBox;
            if (multiSelectTextBox == null)
            {
                return;
            }
            multiSelectTextBox.OnTagModelListPropertyChanged();
        }

        public void OnTagModelListPropertyChanged()
        {
            this.UpdateHasContent();
            this.UpdateTagHostMargin();
        }

        private void RSMultiSelectTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 用户输入文本时，取消Tag的选中状态
            if (this.SelectedTagIndex >= 0 && this.Text.Length > 0)
            {
                ClearTagSelect();
            }
            this.UpdateHasContent();
            this.UpdateTagHostMargin();
        }

        private void UpdateTagHostMargin()
        {
            if (PART_ContentHost == null || PART_TagHost == null)
            {
                return;
            }
            var totalContentWidth = PART_ContentHost.ExtentWidth + 50;
            var actualWidth = this.ActualWidth;
            var tagHostActualWidth = PART_TagHost.ActualWidth;
            double btnClearActualWidth = 0d;
            if (this.PART_BtnClear.Visibility == Visibility.Visible)
            {
                btnClearActualWidth = PART_BtnClear.ActualWidth;
            }
            var totalWidth = (tagHostActualWidth + totalContentWidth + btnClearActualWidth);
            var marginLeft = Math.Min(0, actualWidth - totalWidth);
            //必须设置最大最小值 不然会无限出发sizechanged事件
            marginLeft = Math.Max(-tagHostActualWidth, marginLeft);
            this.PART_TagHost.Margin = new Thickness(marginLeft, 0, 0, 0);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_ContentHost = this.GetTemplateChild(nameof(PART_ContentHost)) as ScrollViewer;
            this.PART_TagHost = this.GetTemplateChild(nameof(PART_TagHost)) as ItemsControl;
            this.PART_BtnClear = this.GetTemplateChild(nameof(PART_BtnClear)) as Button;

            if (this.PART_BtnClear != null)
            {
                this.PART_BtnClear.Click -= PART_BtnClear_Click;
                this.PART_BtnClear.Click += PART_BtnClear_Click;
            }
            if (this.PART_TagHost != null)
            {
                this.PART_TagHost.SizeChanged -= PART_TagHost_SizeChanged;
                this.PART_TagHost.SizeChanged += PART_TagHost_SizeChanged;
            }
        }

        private void PART_TagHost_SizeChanged(object? sender, EventArgs e)
        {
            this.UpdateTagHostMargin();
        }

        private void PART_BtnClear_Click(object sender, RoutedEventArgs e)
        {
            List<TagModel> tagModels = this.TagModelList.ToList();
            this.Clear();
            this.TagModelList.Clear();
            this.UpdateHasContent();
            this.PART_TagHost.Margin = new Thickness(0, 0, 0, 0);
            OnTagModelDeleteCallBack?.Invoke(tagModels);
        }





        private void UpdateHasContent()
        {
            var hasContent = this.Text.Length > 0 || this.TagModelList.Count > 0;
            this.HasContent = hasContent;
        }

        private void ClearTagSelect()
        {
            this.SelectedTagIndex = -1;
            foreach (var tag in this.TagModelList)
            {
                tag.IsSelect = false;
            }
        }

        /// <summary>
        /// 选中指定索引的Tag，-1表示取消所有选中
        /// </summary>
        private void SelectTag(int index)
        {
            if (this.TagModelList == null || this.TagModelList.Count == 0)
            {
                this.SelectedTagIndex = -1;
                return;
            }

            // 确保索引在有效范围内
            if (index < -1)
            {
                index = -1;
            }
            else if (index >= this.TagModelList.Count)
            {
                index = this.TagModelList.Count - 1;
            }

            this.SelectedTagIndex = index;
            UpdateTagSelectionVisual();
        }

        /// <summary>
        /// 根据SelectedTagIndex更新所有Tag的选中状态视觉效果
        /// </summary>
        private void UpdateTagSelectionVisual()
        {
            for (int i = 0; i < this.TagModelList.Count; i++)
            {
                this.TagModelList[i].IsSelect = (i == this.SelectedTagIndex);
            }
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
            int deletedIndex = this.SelectedTagIndex;
            
            this.TagModelList.RemoveAt(deletedIndex);
            this.UpdateHasContent();
            OnTagModelDeleteCallBack?.Invoke(new List<TagModel>() { tagToDelete });

            // 删除后调整选中索引
            if (this.TagModelList.Count == 0)
            {
                this.SelectedTagIndex = -1;
            }
            else if (deletedIndex >= this.TagModelList.Count)
            {
                // 如果删除的是最后一个，选中新的最后一个
                SelectTag(this.TagModelList.Count - 1);
            }
            else
            {
                // 保持当前索引位置（选中下一个Tag）
                SelectTag(deletedIndex);
            }
        }
    }
}
