using Microsoft.Xaml.Behaviors;
using RS.Widgets.Controls;
using RS.Widgets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design.Behavior;

namespace RS.WPFClient.Behaviors
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
                Console.WriteLine("触发焦点");
                if (behavior.AssociatedObject is TextBox textBox)
                {
                    textBox.CaretIndex = behavior.GetCaretIndex(textBox.Text);
                }
                else if (behavior.AssociatedObject is PasswordBox passwordBox)
                {
                    passwordBox.ReflectionCall("Select", passwordBox.Password.Length, 0 );
                }
                behavior.OnFocusedChanged();
            }
        }

        private int GetCaretIndex(string text)
        {
            int caretIndex = 0;
            if (!string.IsNullOrEmpty(text))
            {
                caretIndex = text.Length;
            }
            return caretIndex;
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
