using CommunityToolkit.Mvvm.Input;
using Org.BouncyCastle.Pqc.Crypto.Lms;
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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ZXing.OneD;

namespace RS.Widgets.Controls
{
    public class RSMultiSelectComboBox : DataGrid
    {
        private RSMultiSelectTextBox PART_MultiSelectTextBox;
        private Window ParentWindow;
        static RSMultiSelectComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RSMultiSelectComboBox), new FrameworkPropertyMetadata(typeof(RSMultiSelectComboBox)));
        }

        public RSMultiSelectComboBox()
        {
            this.Loaded += RSMultiSelectComboBox_Loaded;
            this.SelectionChanged += RSMultiSelectComboBox_SelectionChanged;

        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //switch (e.Property.Equals(DataGrid.Colomns))
            //{
            //    default:
            //        break;
            //}

        }
       

        private void RSMultiSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedItems == null || this.SelectedItems.Count == 0)
            {
                return;
            }

            var multiSelectedItems = this.MultiSelectedItems as IList;
            multiSelectedItems?.Clear();
            List<TagModel> tagModelList = new List<TagModel>();
            foreach (var item in this.SelectedItems)
            {
                var tagModel = new TagModel()
                {
                    TagContent = GetDisplayMemberValue(item, this.TagDisplayMemberPath) ?? item,
                    IsSelect = false,
                    Data = item
                };
                tagModelList.Add(tagModel);
                if (multiSelectedItems != null)
                {
                    multiSelectedItems.Add(item);
                }
            }
            this.TagModelList = new ObservableCollection<TagModel>(tagModelList);
            this.PART_MultiSelectTextBox.TagModelList = new ObservableCollection<TagModel>(tagModelList);
        }

        private void RSMultiSelectComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ParentWindow = Window.GetWindow(this);
            if (ParentWindow != null)
            {
                ParentWindow.PreviewMouseLeftButtonUp += ParentWindow_PreviewMouseLeftButtonUp;
            }
        }




        private void ParentWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.PART_MultiSelectTextBox == null)
            {
                return;
            }
            //如果鼠标在这个控件上面则不用处理
            if (this.IsMouseOver)
            {
                return;
            }

            this.IsDropDownOpen = false;
            this.PART_MultiSelectTextBox.SetTagHostMarginZero();
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
            DependencyProperty.Register(nameof(TagModelList), typeof(ObservableCollection<TagModel>), typeof(RSMultiSelectComboBox), new FrameworkPropertyMetadata(new ObservableCollection<TagModel>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(RSMultiSelectComboBox), new PropertyMetadata(false));



        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(RSMultiSelectComboBox), new PropertyMetadata(300D));



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_MultiSelectTextBox = this.GetTemplateChild(nameof(PART_MultiSelectTextBox)) as RSMultiSelectTextBox;

            if (this.PART_MultiSelectTextBox != null)
            {
                this.PART_MultiSelectTextBox.PreviewMouseLeftButtonUp -= PART_MultiSelectTextBox_PreviewMouseLeftButtonUp;
                this.PART_MultiSelectTextBox.PreviewMouseLeftButtonUp += PART_MultiSelectTextBox_PreviewMouseLeftButtonUp;

                this.PART_MultiSelectTextBox.OnTagModelDeleteCallBack -= PART_MultiSelectTextBox_OnTagModelDeleteCallBack;
                this.PART_MultiSelectTextBox.OnTagModelDeleteCallBack += PART_MultiSelectTextBox_OnTagModelDeleteCallBack;


                this.PART_MultiSelectTextBox.RemoveMultiSelectTextBox_LostFocusEvent();
                this.PART_MultiSelectTextBox.LostFocus -= PART_MultiSelectTextBox_LostFocus;
                this.PART_MultiSelectTextBox.LostFocus += PART_MultiSelectTextBox_LostFocus;

                this.PART_MultiSelectTextBox.KeyDown -= PART_MultiSelectTextBox_KeyDown;
                this.PART_MultiSelectTextBox.KeyDown += PART_MultiSelectTextBox_KeyDown;
            }


        }



        private void PART_MultiSelectTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void PART_MultiSelectTextBox_OnSelectTextBoxLostFocus(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("PART_MultiSelectTextBox_OnSelectTextBoxLostFocus");


        }

        private void PART_MultiSelectTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.AddCustomTagModel();
                    break;
            }
        }



        /// <summary>
        /// 添加自定义的标签
        /// </summary>
        private void AddCustomTagModel()
        {
            //这里实现如果用户在输入框里输入了内容，失去焦点或者按下回车键后自动添加一个标签
            var textInput = this.PART_MultiSelectTextBox.Text;
            if (string.IsNullOrEmpty(textInput))
            {
                return;
            }

            var tagModel = new TagModel()
            {
                TagContent = textInput,
                Data = null,
                IsSelect = false
            };
            this.PART_MultiSelectTextBox.TagModelList.Add(tagModel);
            this.TagModelList.Add(tagModel);

            this.PART_MultiSelectTextBox.OnTagModelListPropertyChanged();

            //最后清除数据
            this.PART_MultiSelectTextBox.Clear();
        }

        private void PART_MultiSelectTextBox_OnTagModelDeleteCallBack(List<TagModel> tagModelList)
        {
            var selectItemList = this.SelectedItems as IList;
            if (selectItemList == null || selectItemList.Count == 0)
            {
                return;
            }
            foreach (var tagModel in tagModelList)
            {
                selectItemList.Remove(tagModel.Data);
                this.TagModelList.Remove(tagModel);
                this.SelectedItems.Remove(tagModel.Data);
            }
        }

        private void PART_MultiSelectTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver != this.PART_MultiSelectTextBox)
            {
                return;
            }
            this.IsDropDownOpen = !this.IsDropDownOpen;
            if (!this.IsDropDownOpen)
            {
                this.PART_MultiSelectTextBox.Focus();
            }
        }


        //public object? GetDisplayMemberValue()
        //{
        //    return GetDisplayMemberValue(SelectedItem);
        //}


        public object? GetDisplayMemberValue(object? item, string displayMemberPath)
        {
            if (item == null || string.IsNullOrEmpty(displayMemberPath))
            {
                return null;
            }

            try
            {
                // 解析属性路径（支持嵌套属性，如"User.Name"）
                return GetPropertyValue(item, displayMemberPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解析属性失败：{ex.Message}");
                return null;
            }
        }

        private object? GetPropertyValue(object? obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            var propertyNames = propertyPath.Split('.');
            var currentObj = obj;

            foreach (var propName in propertyNames)
            {
                if (currentObj == null)
                {
                    return null;
                }

                Type type = currentObj.GetType();
                if (type.IsValueType && currentObj is ValueType)
                {
                    PropertyInfo? prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (prop == null)
                    {
                        throw new MissingMemberException($"Property {propName} not found in type {type.Name}");
                    }
                    currentObj = prop.GetValue(currentObj);
                }
                else
                {
                    PropertyInfo? prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (prop == null)
                    {
                        throw new MissingMemberException($"Property {propName} not found in type {type.Name}");
                    }
                    currentObj = prop.GetValue(currentObj);
                }
            }

            return currentObj;
        }
    }
}
