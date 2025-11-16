using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RS.Widgets.Controls
{
    public static class ComboBoxHelper
    {
        
        public static readonly DependencyProperty SelectionBoxItemTemplateProperty =
            DependencyProperty.RegisterAttached(
                "SelectionBoxItemTemplate",
                typeof(DataTemplate),
                typeof(ComboBoxHelper),
                new PropertyMetadata(null));
        public static DataTemplate GetSelectionBoxItemTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(SelectionBoxItemTemplateProperty);
        }

        public static void SetSelectionBoxItemTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(SelectionBoxItemTemplateProperty, value);
        }



        public static readonly DependencyProperty DisplayTextProperty =
           DependencyProperty.RegisterAttached(
               "DisplayText",
               typeof(string),
               typeof(ComboBoxHelper),
               new PropertyMetadata(null));
        public static string GetDisplayText(DependencyObject obj)
        {
            return (string)obj.GetValue(DisplayTextProperty);
        }

        public static void SetDisplayText(DependencyObject obj, string value)
        {
            obj.SetValue(DisplayTextProperty, value);
        }
    }
}
