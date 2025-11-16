using RS.Widgets.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RS.Widgets.Controls
{
    public class RSComboBox : ComboBox
    {
        static RSComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RSComboBox), new FrameworkPropertyMetadata(typeof(RSComboBox)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
