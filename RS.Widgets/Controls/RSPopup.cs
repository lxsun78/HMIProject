using RS.Win32API;
using RS.Win32API.Structs;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
namespace RS.Widgets.Controls
{
    public class RSPopup : Popup
    {
        private HwndSource HwndSource;
        private Window ParentWindow;
        /// <summary> 
        /// 加载窗口随动事件 
        /// </summary> 
        public RSPopup()
        {
            this.Loaded += RSPopup_Loaded;
            this.Opened += RSPopup_Opened;
            this.Closed += RSPopup_Closed;
            this.Unloaded += RSPopup_Unloaded;
            this.StaysOpen = true;
        }

        private void RSPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ParentWindow != null)
            {
                ParentWindow.LocationChanged -= ParentWindow_LocationChanged;
                ParentWindow.SizeChanged -= ParentWindow_SizeChanged;
                ParentWindow.PreviewMouseLeftButtonUp -= ParentWindow_PreviewMouseLeftButtonUp;
            }
        }

        public UIElement RelativeElement
        {
            get { return (UIElement)GetValue(RelativeElementProperty); }
            set { SetValue(RelativeElementProperty, value); }
        }

        public static readonly DependencyProperty RelativeElementProperty =
            DependencyProperty.Register("RelativeElement", typeof(UIElement), typeof(RSPopup), new PropertyMetadata(null));



        private void RSPopup_Closed(object? sender, EventArgs e)
        {
        }

        private void RSPopup_Opened(object? sender, EventArgs e)
        {

        }

        private void RSPopup_Loaded(object sender, RoutedEventArgs e)
        {
            ParentWindow = this.TryFindParent<RSWindow>();
            if (ParentWindow != null)
            {
                ParentWindow.LocationChanged -= ParentWindow_LocationChanged;
                ParentWindow.LocationChanged += ParentWindow_LocationChanged;
                ParentWindow.SizeChanged -= ParentWindow_SizeChanged;
                ParentWindow.SizeChanged += ParentWindow_SizeChanged;
                ParentWindow.PreviewMouseLeftButtonUp -= ParentWindow_PreviewMouseLeftButtonUp;
                ParentWindow.PreviewMouseLeftButtonUp += ParentWindow_PreviewMouseLeftButtonUp;
            }
        }



        private void ParentWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var directlyOver = Mouse.DirectlyOver as UIElement;
            if (this.IsMouseOver || directlyOver == this.RelativeElement)
            {
                return;
            }

            if (this.IsOpen)
            {
                Console.WriteLine("ParentWindow_PreviewMouseLeftButtonUp-PopClose");
                this.SetCurrentValue(IsOpenProperty, false);
            }
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdatePopupPosition();
        }

        /// <summary> 
        /// 刷新位置 
        /// </summary> 
        private void ParentWindow_LocationChanged(object? sender, EventArgs e)
        {
            this.UpdatePopupPosition();
        }

        private void UpdatePopupPosition()
        {
            if (this.IsOpen)
            {
                _ = this.ReflectionCall("UpdatePosition");
            }
        }


        public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(typeof(Popup), new FrameworkPropertyMetadata(false, OnTopmostPropertyChanged));
        public bool Topmost
        {
            get { return (bool)GetValue(TopmostProperty); }
            set { SetValue(TopmostProperty, value); }
        }

        private static void OnTopmostPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RSPopup)?.UpdateWindow();
        }

        /// <summary>  
        /// 重写
        /// </summary>  
        /// <param name="e"></param>  
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            UpdateWindow();
        }

        /// <summary>  
        /// 更新Popup窗体位置
        /// </summary>  
        private void UpdateWindow()
        {
            var handle = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;

            RECT lpRect = new RECT();
            if (NativeMethods.IntGetWindowRect(new HandleRef(null, handle), ref lpRect))
            {
                NativeMethods.SetWindowPos(new HandleRef(null, handle), new HandleRef(null, Topmost ? -1 : -2), lpRect.Left, lpRect.Top, (int)this.Width, (int)this.Height, 0);
            }
        }
    }
}
