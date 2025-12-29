using CommunityToolkit.Mvvm.Input;
using RS.Commons.Extensions;
using RS.Widgets.Models;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ZXing;
using ZXing.OneD;

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
           
            if (model==null)
            {
                return;
            }
            ClearTagSelect();
            model.IsSelect = true;

            //var itemCollection = this.PART_TagHost.Items;
            //var stackPanel = this.PART_TagHost.FindChild<StackPanel>();
            //var elementCollection = stackPanel.Children;
            //foreach (var item in elementCollection)
            //{

            //}
            //Console.WriteLine(13123123123123);
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


        private void RSMultiSelectTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Back:
                    if (this.Text.Length > 0)
                    {
                        return;
                    }
                    if (this.TagModelList.Count == 0)
                    {
                        return;
                    }
                    this.TagModelList.RemoveAt(this.TagModelList.Count - 1);
                    this.UpdateHasContent();
                    break;

                case Key.Left:
                    Console.WriteLine("Key.Left");
                    break;
                case Key.Right:
                    Console.WriteLine("Key.Right");
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
            foreach (var tag in this.TagModelList)
            {
                tag.IsSelect = false;
            }
        }
    }
}
