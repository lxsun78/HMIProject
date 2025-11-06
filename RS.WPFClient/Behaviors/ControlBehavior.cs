using Microsoft.Xaml.Behaviors;
using RS.Widgets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design.Behavior;

namespace RS.WPFClient.Client.Behaviors
{
    public class ControlBehavior : Behavior<Control>
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
                typeof(ControlBehavior),
                new PropertyMetadata(false, OnIsFocusedChanged)
            );


        public static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ControlBehavior)d;
            if (behavior.AssociatedObject != null && (bool)e.NewValue)
            {
                behavior.AssociatedObject.Focus();
               

                if (behavior.AssociatedObject is TextBox textBox)
                {
                    textBox.SelectAll();
                }
                else if (behavior.AssociatedObject is PasswordBox passwordBox)
                {
                    passwordBox.SelectAll();
                }
                behavior.OnFocusedChanged();
              
            }
        }
    

        public virtual void OnFocusedChanged()
        {

        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.LostFocus += AssociatedObject_LostFocus;
        }

        private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsFocused = false;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
            base.OnDetaching();
        }
    }
}
