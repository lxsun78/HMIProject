using Microsoft.Xaml.Behaviors;
using RS.WPFClient.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RS.WPFClient.Behaviors
{
    public class SearchViewBehavior : Behavior<SearchView>
    {
        public ICommand HideSearchCommand
        {
            get { return (ICommand)GetValue(HideSearchCommandProperty); }
            set { SetValue(HideSearchCommandProperty, value); }
        }

        public static readonly DependencyProperty HideSearchCommandProperty =
            DependencyProperty.Register("HideSearchCommand", typeof(ICommand), typeof(SearchViewBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this.AssociatedObject);
            window.PreviewMouseLeftButtonUp += Window_MouseLeftButtonUp;
            window.Deactivated += Window_Deactivated;
        }
    
        private void Window_Deactivated(object? sender, EventArgs e)
        {
            HideSearchCommand?.Execute(null);
        }

        private void Window_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var source = e.Source;
            if (source==this.AssociatedObject)
            {
                return;
            }
            HideSearchCommand?.Execute(null);
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnDetaching();
        }
    }
}
