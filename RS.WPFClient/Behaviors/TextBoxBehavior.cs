using Microsoft.Xaml.Behaviors;
using RS.Widgets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RS.WPFClient.Client.Behaviors
{
    public class TextBoxBehavior : Behavior<TextBox>
    {
       
        public bool IsFocused
        {
            get
            {
                return (bool)GetValue(IsFocusedProperty);
            }
            set
            {
                SetValue(IsFocusedProperty, value);
            }
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(TextBoxBehavior),
                new PropertyMetadata(false, OnIsFocusedChanged)
            );


        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (TextBoxBehavior)d;
            if (behavior.AssociatedObject != null && (bool)e.NewValue)
            {
                behavior.AssociatedObject.Focus();
                behavior.AssociatedObject.SelectAll();
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
