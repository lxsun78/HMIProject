using Microsoft.Extensions.DependencyInjection;
using NPOI.XWPF.UserModel;
using RS.Commons.Attributs;
using RS.Commons.Helper;
using RS.Widgets.Controls;
using RS.WPFClient.IServices;
using RS.WPFClient.ViewModels;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;


namespace RS.WPFClient.Views
{

    /// <summary>
    /// 位置枚举，涵盖常用的方位描述
    /// </summary>
    public enum Position
    {

        /// <summary>
        /// 未知
        /// </summary>
        Unknown,

        /// <summary>
        /// 左侧位置
        /// </summary>
        Left,

        /// <summary>
        /// 右侧位置
        /// </summary>
        Right,

        /// <summary>
        /// 上方位置
        /// </summary>
        Top,

        /// <summary>
        /// 下方位置
        /// </summary>
        Bottom,

        /// <summary>
        /// 左上位置（左+上）
        /// </summary>
        TopLeft,

        /// <summary>
        /// 左下位置（左+下）
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 右上位置（右+上）
        /// </summary>
        TopRight,

        /// <summary>
        /// 右下位置（右+下）
        /// </summary>
        BottomRight
    }





    [ServiceInjectConfig(ServiceLifetime.Singleton)]
    public partial class HomeView : RSWindow
    {
        public HomeViewModel ViewModel { get; set; }
        public HomeView()
        {
            InitializeComponent();
            this.Loaded += HomeView_Loaded;
        }

        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = this.DataContext as HomeViewModel;
        }




        private bool IsCanvas_MouseLeftButtonDown = false;
        private Point MouseDownPosition;
        private Point MouseDownCaretPosition;
        private TextBlock TextInput;
        private StringBuilder TextStringBuilder;
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsCanvas_MouseLeftButtonDown = true;

            MouseDownPosition = e.GetPosition(sender as IInputElement);
            MouseDownCaretPosition= UpdateCaretPosition(MouseDownPosition);


            TextInput = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
            };


            TextInput.Loaded += TextInput_Loaded;

            this.TestCanvas.Children.Add(TextInput);
            TextStringBuilder = new StringBuilder();

            this.TestCanvas.Focus();
        }

        private void TextInput_Loaded(object sender, RoutedEventArgs e)
        {

            var textBlock = sender as TextBlock;
            var left = this.ViewModel.CaretLeft;
            var top = this.ViewModel.CaretTop;
            var caretHeight = this.ViewModel.CaretHeight;
            var caretWidth = this.ViewModel.CaretWidth;
            var actualWidth = textBlock.ActualWidth;
            var actualHeight = textBlock.ActualHeight;
            var leftShould = left + caretWidth;
            var topShould = top + caretHeight / 2 - actualHeight / 2;
            Canvas.SetLeft(TextInput, leftShould);
            Canvas.SetTop(TextInput, topShould);
        }

        private void TestCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsCanvas_MouseLeftButtonDown = false;

            if (ContentSelectDecorate != null && this.TestCanvas.Children.Contains(ContentSelectDecorate))
            {
                this.TestCanvas.Children.Remove(ContentSelectDecorate);
            }
            this.ContentSelectDecorate = null;
        }
        private Rectangle ContentSelectDecorate;
        private void TestCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var currentPosition = e.GetPosition(this.TestCanvas);
            var vector = currentPosition - MouseDownPosition;
            //if (this.IsCanvas_MouseLeftButtonDown
            //    && (Math.Abs(vector.X) >= SystemParameters.MinimumHorizontalDragDistance
            //    || Math.Abs(vector.Y) >= SystemParameters.MinimumVerticalDragDistance))
            //{
            //    if (ContentSelectDecorate == null)
            //    {
            //        ContentSelectDecorate = new Rectangle()
            //        {
            //            Fill = Brushes.Brown,
            //            MinHeight= Math.Max(Math.Abs(vector.Y), 15)
            //        };
            //        Panel.SetZIndex(ContentSelectDecorate, -1);
            //        Canvas.SetLeft(ContentSelectDecorate, MouseDownCaretPosition.X);
            //        Canvas.SetTop(ContentSelectDecorate, MouseDownCaretPosition.Y);
            //        this.TestCanvas.Children.Add(ContentSelectDecorate);
            //    }
            //    Console.WriteLine($"vector:{vector}");
            //    ContentSelectDecorate.Width = Math.Abs(vector.X);
            //    ContentSelectDecorate.Height = Math.Max(Math.Abs(vector.Y), 15);
            //    var position = GetScreenRelativePosition(MouseDownPosition, currentPosition);
            //    Console.WriteLine(position.ToString());


            //    var contentSelectDecorateLeft = Canvas.GetLeft(ContentSelectDecorate);
            //    var contentSelectDecorateTop = Canvas.GetTop(ContentSelectDecorate);

            //    var bottomRight = new Point(contentSelectDecorateLeft + ContentSelectDecorate.Width,
            //        contentSelectDecorateTop + ContentSelectDecorate.Height);

            //    this.ViewModel.CaretLeft = bottomRight.X;
            //    this.ViewModel.CaretTop = bottomRight.Y- this.ViewModel.CaretHeight;
            //}

            if (this.IsCanvas_MouseLeftButtonDown)
            {
                UpdateCaretPosition(currentPosition);
            }
        }



        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key);
        }


        private void TestCanvas_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("TestCanvas_GotFocus");
        }

        private void TestCanvas_LostFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("TestCanvas_LostFocus");
        }

        private void TestCanvas_TextInput(object sender, TextCompositionEventArgs e)
        {
            TextStringBuilder.Append(e.Text);
            this.TextInput.LayoutUpdated += TextInput_LayoutUpdated;
            this.TextInput.Text = TextStringBuilder.ToString();
        }

        private void TextInput_LayoutUpdated(object? sender, EventArgs e)
        {
            var height = this.TextInput.ActualHeight;
            var width = this.TextInput.ActualWidth;
            var textInputLeft = Canvas.GetLeft(this.TextInput);
            var textInputTop = Canvas.GetTop(this.TextInput);
            this.ViewModel.CaretLeft = textInputLeft + width;
        }

        private Point UpdateCaretPosition(Point caretPosition)
        {
            var positionX = caretPosition.X;
            var positionY = caretPosition.Y;

            var height = this.ViewModel.CaretHeight;
            var width = this.ViewModel.CaretWidth;

            var halfHeight = height / 2;
            var halfWidth = width / 2;
            var left = positionX - halfWidth;
            var top = positionY - halfHeight;


            this.ViewModel.CaretLeft = left;
            this.ViewModel.CaretTop = top;

            return new Point(left, top);
        }
       


        private const double Tolerance = 1e-9;

        public static Position GetScreenRelativePosition(Point referencePoint, Point currentPoint)
        {
            if (Math.Abs(currentPoint.X - referencePoint.X) < Tolerance &&
                Math.Abs(currentPoint.Y - referencePoint.Y) < Tolerance)
            {
                return Position.Unknown;
            }

            bool isLeft = currentPoint.X < referencePoint.X - Tolerance;
            bool isRight = currentPoint.X > referencePoint.X + Tolerance;
            bool isTop = currentPoint.Y < referencePoint.Y - Tolerance;
            bool isBottom = currentPoint.Y > referencePoint.Y + Tolerance;

            if (isLeft && isTop)
            {
                return Position.TopLeft;
            }
            if (isLeft && isBottom)
            {
                return Position.BottomLeft;
            }
            if (isRight && isTop)
            {
                return Position.TopRight;
            }
            if (isRight && isBottom)
            {
                return Position.BottomRight;
            }
            if (isLeft)
            {
                return Position.Left;
            }
            if (isRight)
            {
                return Position.Right;
            }
            if (isTop)
            {
                return Position.Top;
            }
            return Position.Bottom;
        }

    }
}
