using RS.Commons.Enums;
using RS.Widgets.Models;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing;


namespace RS.Widgets.Controls
{
    internal class QRCodeLoginTask : IDisposable
    {
        private DispatcherTimer? QueryQRCodeLoginStatusAsyncFuncDispatcherTimer;
        private QRCodeLoginResultModel? LoginQRCodeResult;
        private RSQRCodeLogin? RSQRCodeLogin;
        public Action<QRCodeLoginResultModel>? CancelQRCodeLoginAction;
        public Action<QRCodeLoginResultModel>? QRCodeAuthLoginSuccessAction;
        public Func<Task<QRCodeLoginResultModel>>? GetLoginQRCodeAsyncFunc;
        public Func<Task<QRCodeLoginStatusModel>>? QueryQRCodeLoginStatusAsyncFunc;
        private bool IsEndQRCodeLogin = false;


        public QRCodeLoginTask(RSQRCodeLogin rSQRCodeLogin)
        {
            RSQRCodeLogin = rSQRCodeLogin;
        }

        ~QRCodeLoginTask()
        {
            Dispose();
        }

        public async void BeginQRCodeLogin()
        {
            if (RSQRCodeLogin == null)
            {
                return;
            }

            RSQRCodeLogin.QRCodeLoginStatus = QRCodeLoginStatusEnum.BeginGetQRCode;
            this.GenerateQRCodeImgSource(" ", RSQRCodeLogin.QRCodeWidth, RSQRCodeLogin.QRCodeHeight);

            if (this.GetLoginQRCodeAsyncFunc == null)
            {
                return;
            }

            this.LoginQRCodeResult = await this.GetLoginQRCodeAsyncFunc.Invoke();
            if (IsEndQRCodeLogin)
            {
                return;
            }

            if (this.LoginQRCodeResult != null && this.LoginQRCodeResult.IsSuccess)
            {

                this.GenerateQRCodeImgSource(this.LoginQRCodeResult.QRCodeContent, RSQRCodeLogin.QRCodeWidth, RSQRCodeLogin.QRCodeHeight);
                if (!IsEndQRCodeLogin)
                {
                    this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer = new DispatcherTimer();
                    this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer.Tick += QueryQRCodeLoginStatusAsyncFuncDispatcherTimer_Tick; ;
                    this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
                    this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer.Start();
                }
                if (!IsEndQRCodeLogin)
                {
                    QueryQRCodeLoginStatusAsyncFuncDispatcherTimer_Tick(null, new EventArgs());
                }
            }
            else
            {
                if (!IsEndQRCodeLogin)
                {
                    RSQRCodeLogin.QRCodeLoginStatus = QRCodeLoginStatusEnum.QRCodeLoginTimeOut;
                }
            }
        }


        public void EndQRCodeLogin()
        {
            IsEndQRCodeLogin = true;
            QueryQRCodeLoginStatusAsyncFuncDispatcherTimer?.Stop();
            QueryQRCodeLoginStatusAsyncFuncDispatcherTimer = null;
        }


        private async void QueryQRCodeLoginStatusAsyncFuncDispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (RSQRCodeLogin == null)
            {
                return;
            }
            if (this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer == null)
            {
                return;
            }
            if (this.LoginQRCodeResult == null)
            {
                this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer.Stop();
                return;
            }

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(this.LoginQRCodeResult.ExpireTime);
            var expireTime = dateTimeOffset.LocalDateTime;

            if (DateTime.Now >= expireTime.AddSeconds(-1))
            {
                this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer.Stop();
                if (!IsEndQRCodeLogin)
                {
                    RSQRCodeLogin.QRCodeLoginStatus = QRCodeLoginStatusEnum.QRCodeLoginTimeOut;
                }
                return;
            }

            if (!IsEndQRCodeLogin)
            {
                QRCodeLoginStatusModel? qRCodeLoginStatusModel = null;
                if (this.QueryQRCodeLoginStatusAsyncFunc != null)
                {
                    qRCodeLoginStatusModel = await QueryQRCodeLoginStatusAsyncFunc.Invoke();
                }

                if (qRCodeLoginStatusModel != null && qRCodeLoginStatusModel.IsSuccess)
                {
                    if (!IsEndQRCodeLogin)
                    {
                        RSQRCodeLogin.QRCodeLoginStatus = qRCodeLoginStatusModel.QRCodeLoginStatus;
                    }

                    switch (RSQRCodeLogin.QRCodeLoginStatus)
                    {
                        case QRCodeLoginStatusEnum.BeginGetQRCode:
                            break;
                        case QRCodeLoginStatusEnum.WaitScanQRCode:
                            break;
                        case QRCodeLoginStatusEnum.ScanQRCodeSuccess:
                            break;
                        case QRCodeLoginStatusEnum.QRCodeAuthLogin:
                            this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer.Stop();
                            if (!IsEndQRCodeLogin)
                            {
                                QRCodeAuthLoginSuccessAction?.Invoke(this.LoginQRCodeResult);
                            }

                            break;
                        case QRCodeLoginStatusEnum.CancelQRCodeLoginAction:
                            this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer.Stop();
                            if (!IsEndQRCodeLogin)
                            {
                                CancelQRCodeLoginAction?.Invoke(this.LoginQRCodeResult);
                            }
                            break;
                        case QRCodeLoginStatusEnum.QRCodeLoginTimeOut:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (!IsEndQRCodeLogin)
                    {
                        RSQRCodeLogin.QRCodeLoginStatus = QRCodeLoginStatusEnum.QRCodeLoginTimeOut;
                    }
                }
                if (IsEndQRCodeLogin)
                {
                    this.QueryQRCodeLoginStatusAsyncFuncDispatcherTimer.Stop();
                }
            }
        }

        private void GenerateQRCodeImgSource(string qRCodeContent, double qRCodeWidth, double qRCodeHeight)
        {
            if (RSQRCodeLogin == null)
            {
                return;
            }

            BarcodeWriterPixelData barcodeWriterPixelData = new BarcodeWriterPixelData()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions()
                {
                    Height = (int)qRCodeHeight,
                    Width = (int)qRCodeWidth,
                    Margin = 0
                }
            };

            var pixelData = barcodeWriterPixelData.Write(qRCodeContent);

            double dpiX = 96D; double dpiY = 96D;
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }
            WriteableBitmap writeableBitmap = new WriteableBitmap(pixelData.Width, pixelData.Height, dpiX, dpiY, PixelFormats.Bgra32, null);
            writeableBitmap.WritePixels(new Int32Rect(0, 0, pixelData.Width, pixelData.Height), pixelData.Pixels, pixelData.Width * sizeof(int), 0);

            if (!this.IsEndQRCodeLogin)
            {
                RSQRCodeLogin.QRCodeImgSource = writeableBitmap;
            }
        }

        public void Dispose()
        {
            QueryQRCodeLoginStatusAsyncFuncDispatcherTimer?.Stop();
            GC.SuppressFinalize(this);
        }
    }


    [TemplatePart(Name = nameof(PART_BtnReGetQRCode), Type = typeof(Button))]
    public class RSQRCodeLogin : ContentControl
    {

        private Button? PART_BtnReGetQRCode;

        private QRCodeLoginTask? CurrentQRCodeLoginTask;


        static RSQRCodeLogin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RSQRCodeLogin), new FrameworkPropertyMetadata(typeof(RSQRCodeLogin)));
        }

        public RSQRCodeLogin()
        {
            this.Loaded += RSQRCodeLogin_Loaded;
            this.Unloaded += RSQRCodeLogin_Unloaded;
        }

        private void RSQRCodeLogin_Unloaded(object sender, RoutedEventArgs e)
        {
            this.EndQRCodeLoginTrigger();
        }

        private void RSQRCodeLogin_Loaded(object sender, RoutedEventArgs e)
        {
            this.BeginQRCodeLoginTrigger();
        }

        /// <summary>
        /// 获取登录二维码
        /// </summary>
        public Func<Task<QRCodeLoginResultModel>>? GetLoginQRCodeAsyncFunc
        {
            get { return (Func<Task<QRCodeLoginResultModel>>?)GetValue(GetLoginQRCodeAsyncFuncProperty); }
            set { SetValue(GetLoginQRCodeAsyncFuncProperty, value); }
        }

        public static readonly DependencyProperty GetLoginQRCodeAsyncFuncProperty =
            DependencyProperty.Register("GetLoginQRCodeAsyncFunc", typeof(Func<Task<QRCodeLoginResultModel>>), typeof(RSQRCodeLogin), new PropertyMetadata(null));



        /// <summary>
        /// 取消二维码登录
        /// </summary>
        public Action<QRCodeLoginResultModel>? CancelQRCodeLoginAction
        {
            get { return (Action<QRCodeLoginResultModel>?)GetValue(CancelQRCodeLoginActionProperty); }
            set { SetValue(CancelQRCodeLoginActionProperty, value); }
        }

        public static readonly DependencyProperty CancelQRCodeLoginActionProperty =
            DependencyProperty.Register("CancelQRCodeLoginAction", typeof(Action<QRCodeLoginResultModel>), typeof(RSQRCodeLogin), new PropertyMetadata(null));




        /// <summary>
        /// 查询二维码登录状态
        /// </summary>
        public Func<Task<QRCodeLoginStatusModel>>? QueryQRCodeLoginStatusAsyncFunc
        {
            get { return (Func<Task<QRCodeLoginStatusModel>>?)GetValue(QueryQRCodeLoginStatusAsyncFuncProperty); }
            set { SetValue(QueryQRCodeLoginStatusAsyncFuncProperty, value); }
        }

        public static readonly DependencyProperty QueryQRCodeLoginStatusAsyncFuncProperty =
            DependencyProperty.Register("QueryQRCodeLoginStatusAsyncFunc", typeof(Func<Task<QRCodeLoginStatusModel>>), typeof(RSQRCodeLogin), new PropertyMetadata(null));




        /// <summary>
        /// 二维码授权登录成功
        /// </summary>
        public Action<QRCodeLoginResultModel>? QRCodeAuthLoginSuccessAction
        {
            get { return (Action<QRCodeLoginResultModel>?)GetValue(QRCodeAuthLoginSuccessActionProperty); }
            set { SetValue(QRCodeAuthLoginSuccessActionProperty, value); }
        }

        public static readonly DependencyProperty QRCodeAuthLoginSuccessActionProperty =
            DependencyProperty.Register("QRCodeAuthLoginSuccessAction", typeof(Action<QRCodeLoginResultModel>), typeof(RSQRCodeLogin), new PropertyMetadata(null));



        private void BeginQRCodeLoginTrigger()
        {
            if (CurrentQRCodeLoginTask != null)
            {
                CurrentQRCodeLoginTask.EndQRCodeLogin();
            }

            using (QRCodeLoginTask qRCodeLoginTask = new QRCodeLoginTask(this))
            {
                qRCodeLoginTask.CancelQRCodeLoginAction = this.CancelQRCodeLoginAction;
                qRCodeLoginTask.GetLoginQRCodeAsyncFunc = this.GetLoginQRCodeAsyncFunc;
                qRCodeLoginTask.QRCodeAuthLoginSuccessAction = this.QRCodeAuthLoginSuccessAction;
                qRCodeLoginTask.QueryQRCodeLoginStatusAsyncFunc = this.QueryQRCodeLoginStatusAsyncFunc;
                CurrentQRCodeLoginTask = qRCodeLoginTask;
                qRCodeLoginTask.BeginQRCodeLogin();
            }
        }

        private void EndQRCodeLoginTrigger()
        {
            if (CurrentQRCodeLoginTask != null)
            {
                CurrentQRCodeLoginTask.EndQRCodeLogin();
            }
            CurrentQRCodeLoginTask = null;
        }


        /// <summary>
        /// 二维码宽度
        /// </summary>
        public double QRCodeWidth
        {
            get { return (double)GetValue(QRCodeWidthProperty); }
            set { SetValue(QRCodeWidthProperty, value); }
        }


        public static readonly DependencyProperty QRCodeWidthProperty =
            DependencyProperty.Register("QRCodeWidth", typeof(double), typeof(RSQRCodeLogin), new PropertyMetadata(150D));

        /// <summary>
        /// 二维码高度
        /// </summary>
        public double QRCodeHeight
        {
            get { return (double)GetValue(QRCodeHeightProperty); }
            set { SetValue(QRCodeHeightProperty, value); }
        }


        public static readonly DependencyProperty QRCodeHeightProperty =
            DependencyProperty.Register("QRCodeHeight", typeof(double), typeof(RSQRCodeLogin), new PropertyMetadata(150D));


        /// <summary>
        /// 二维码图片资源
        /// </summary>
        public ImageSource QRCodeImgSource
        {
            get { return (ImageSource)GetValue(QRCodeImgSourceProperty); }
            set { SetValue(QRCodeImgSourceProperty, value); }
        }
        public static readonly DependencyProperty QRCodeImgSourceProperty =
            DependencyProperty.Register("QRCodeImgSource", typeof(ImageSource), typeof(RSQRCodeLogin), new PropertyMetadata(null));
       

        /// <summary>
        /// 二维码登录状态
        /// </summary>
        public QRCodeLoginStatusEnum QRCodeLoginStatus
        {
            get { return (QRCodeLoginStatusEnum)GetValue(QRCodeLoginStatusProperty); }
            set { SetValue(QRCodeLoginStatusProperty, value); }
        }

        public static readonly DependencyProperty QRCodeLoginStatusProperty =
            DependencyProperty.Register("QRCodeLoginStatus", typeof(QRCodeLoginStatusEnum), typeof(RSQRCodeLogin), new PropertyMetadata(QRCodeLoginStatusEnum.BeginGetQRCode));


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_BtnReGetQRCode = GetTemplateChild(nameof(PART_BtnReGetQRCode)) as Button;

            if (this.PART_BtnReGetQRCode is not null)
            {
                this.PART_BtnReGetQRCode.Click -= PART_BtnReGetQRCode_Click;
                this.PART_BtnReGetQRCode.Click += PART_BtnReGetQRCode_Click;
            }
        }

        private void PART_BtnReGetQRCode_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentQRCodeLoginTask?.BeginQRCodeLogin();
        }
    }


}
