using RS.Widgets.Interfaces;
using RS.Widgets.Models;
using RS.Win32API;
using RS.Win32API.Structs;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace RS.Widgets.Controls
{

    public class RSRichTextBox : RichTextBox
    {

        public RSRichTextBox()
        {
            this.Loaded += RSRichTextBox_Loaded;
        }

        private void RSRichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.Document.PagePadding = PagePadding;
        }



        public Thickness PagePadding
        {
            get { return (Thickness)GetValue(PagePaddingProperty); }
            set { SetValue(PagePaddingProperty, value); }
        }

        public static readonly DependencyProperty PagePaddingProperty =
            DependencyProperty.Register("PagePadding", typeof(Thickness), typeof(RSRichTextBox), new PropertyMetadata(new Thickness(2,0,2,0)));



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

    }
}
