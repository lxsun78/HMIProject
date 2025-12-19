using CommunityToolkit.Mvvm.Input;
using RS.Commons.Enums;
using RS.Widgets.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RS.OmniComLib.Controls
{
    public class RSSerialPort : UserControl
    {
        private RSDialog PART_RSDialog;
        private DataGrid? PART_DataGrid;
        private Button PART_BtnConnect;
        private Button PART_BtnDisConnect;
        private Button PART_BtnSaveConfig;
        private RSWindow ParentWin;
        private CancellationTokenSource ConnectCTS;

        public SerialPort SerialPort { get; private set; }

        public RSSerialPort()
        {
            this.SetValue(AddCommandPropertyKey, new RelayCommand<object>(AddData));
            this.SetValue(DeleteCommandPropertyKey, new RelayCommand<string>(DeleteModbusDataConfigModel));
            this.SetValue(ImportConfigCommandPropertyKey, new RelayCommand<object>(ImportConfig));
            this.SetValue(ExportCommandPropertyKey, new RelayCommand<object>(ExportConfig));
            this.SetValue(TemplateDownloadCommandPropertyKey, new RelayCommand<object>(TemplateDownload));
            this.SetValue(CellValueEditChangedCommandPropertyKey, new RelayCommand<string>(CellValueEditChanged));
            Loaded += RSSerialPort_Loaded;
        }


        #region Command事件

        private static readonly DependencyPropertyKey CellValueEditChangedCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CellValueEditChangedCommand), typeof(ICommand), typeof(RSSerialPort), new PropertyMetadata(null));
        public static readonly DependencyProperty CellValueEditChangedCommandProperty = CellValueEditChangedCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// 单元格值变化触发事件
        /// </summary>
        public ICommand CellValueEditChangedCommand
        {
            get { return (ICommand)GetValue(CellValueEditChangedCommandProperty); }
        }


        private static readonly DependencyPropertyKey AddCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(AddCommand), typeof(ICommand), typeof(RSSerialPort), new PropertyMetadata(null));
        public static readonly DependencyProperty AddCommandProperty = AddCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// 新增数据
        /// </summary>
        public ICommand AddCommand
        {
            get { return (ICommand)GetValue(AddCommandProperty); }
        }


        private static readonly DependencyPropertyKey DeleteCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(DeleteCommand), typeof(ICommand), typeof(RSSerialPort), new PropertyMetadata(null));

        public static readonly DependencyProperty DeleteCommandProperty = DeleteCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// 删除数据
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
        }

        // 导入配置命令依赖属性
        private static readonly DependencyPropertyKey ImportConfigCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ImportConfigCommand), typeof(ICommand), typeof(RSSerialPort), new PropertyMetadata(null));
        public static readonly DependencyProperty ImportConfigCommandProperty = ImportConfigCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// 导入配置
        /// </summary>
        public ICommand ImportConfigCommand
        {
            get { return (ICommand)GetValue(ImportConfigCommandProperty); }
        }



        private static readonly DependencyPropertyKey ExportCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ExportCommand), typeof(ICommand), typeof(RSSerialPort), new PropertyMetadata(null));
        public static readonly DependencyProperty ExportCommandProperty = ExportCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// 导出配置
        /// </summary>
        public ICommand ExportCommand
        {
            get { return (ICommand)GetValue(ExportCommandProperty); }
        }

        // 模版下载命令依赖属性
        private static readonly DependencyPropertyKey TemplateDownloadCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TemplateDownloadCommand), typeof(ICommand), typeof(RSSerialPort), new PropertyMetadata(null));
        public static readonly DependencyProperty TemplateDownloadCommandProperty = TemplateDownloadCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// 下载模版
        /// </summary>
        public ICommand TemplateDownloadCommand
        {
            get { return (ICommand)GetValue(TemplateDownloadCommandProperty); }
        }
        #endregion

        #region 依赖属性 这是串口通讯都需要有的基本数据
        [Description("波特率")]
        [DefaultValue(9600)]
        public int BaudRate
        {
            get { return (int)GetValue(BaudRateProperty); }
            set { SetValue(BaudRateProperty, value); }
        }

        public static readonly DependencyProperty BaudRateProperty =
            DependencyProperty.Register("BaudRate", typeof(int), typeof(RSSerialPort), new PropertyMetadata(9600, OnBaudRatePropertyChanged));

        private static void OnBaudRatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        [Description("串口名称")]
        [DefaultValue(null)]
        public string PortName
        {
            get { return (string)GetValue(PortNameProperty); }
            set { SetValue(PortNameProperty, value); }
        }

        public static readonly DependencyProperty PortNameProperty =
            DependencyProperty.Register("PortName", typeof(string), typeof(RSSerialPort), new PropertyMetadata(null));


        [Description("握手协议")]
        [DefaultValue(Handshake.None)]
        public Handshake Handshake
        {
            get { return (Handshake)GetValue(HandshakeProperty); }
            set { SetValue(HandshakeProperty, value); }
        }

        public static readonly DependencyProperty HandshakeProperty =
            DependencyProperty.Register("Handshake", typeof(Handshake), typeof(RSSerialPort), new PropertyMetadata(Handshake.None));


        [Description("停止位")]
        [DefaultValue(StopBits.One)]
        public StopBits StopBits
        {
            get { return (StopBits)GetValue(StopBitsProperty); }
            set { SetValue(StopBitsProperty, value); }
        }

        public static readonly DependencyProperty StopBitsProperty =
            DependencyProperty.Register("StopBits", typeof(StopBits), typeof(RSSerialPort), new PropertyMetadata(StopBits.One));


        [Description("数据位")]
        [DefaultValue(8)]
        public int DataBits
        {
            get { return (int)GetValue(DataBitsProperty); }
            set { SetValue(DataBitsProperty, value); }
        }

        public static readonly DependencyProperty DataBitsProperty =
            DependencyProperty.Register("DataBits", typeof(int), typeof(RSSerialPort), new PropertyMetadata(8));


        [Description("奇偶校验")]
        [DefaultValue(Parity.None)]
        public Parity Parity
        {
            get { return (Parity)GetValue(ParityProperty); }
            set { SetValue(ParityProperty, value); }
        }

        public static readonly DependencyProperty ParityProperty =
            DependencyProperty.Register("Parity", typeof(Parity), typeof(RSSerialPort), new PropertyMetadata(Parity.None));

        #endregion


        #region 这是串口通用数据

        private static List<string> serialPortNameList;
        /// <summary>
        /// 获取串口设备名称
        /// </summary>
        public static List<string> SerialPortNameList
        {
            get
            {
                if (serialPortNameList == null)
                {
                    serialPortNameList = GetAvailablePorts().ToList();
                    serialPortNameList.Sort();
                }
                return serialPortNameList;
            }
        }



        private static List<int> dataBitsList;
        /// <summary>
        ///  数据位
        /// </summary>
        public static List<int> DataBitsList
        {
            get
            {
                if (dataBitsList == null)
                {
                    dataBitsList = new List<int>()
                    {
                      5,6,7,8
                    };
                }
                return dataBitsList;
            }
        }

        private static List<int> baudRateList;
        /// <summary>
        /// 波特率
        /// </summary>
        public static List<int> BaudRateList
        {
            get
            {
                if (baudRateList == null)
                {
                    baudRateList = new List<int>()
                    {
                      1200,2400,4800,9600,19200,38400,57600,115200
                    };
                }
                return baudRateList;
            }
        }


        private static List<Parity> parityList;
        /// <summary>
        /// 奇偶校验位
        /// </summary>
        public static List<Parity> ParityList
        {
            get
            {
                if (parityList == null)
                {
                    parityList = Enum.GetValues<Parity>().ToList();
                }
                return parityList;
            }
        }


        private static List<StopBits> stopBitsList;
        /// <summary>
        /// 停止位
        /// </summary>
        public static List<StopBits> StopBitsList
        {
            get
            {
                if (stopBitsList == null)
                {
                    stopBitsList = Enum.GetValues<StopBits>().ToList();
                }
                return stopBitsList;
            }
        }
        #endregion


        /// <summary>
        /// 获取可用串口列表
        /// </summary>
        /// <returns>串口名称列表</returns>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }


        /// <summary>
        /// 应用模版
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_RSDialog = GetTemplateChild(nameof(PART_RSDialog)) as RSDialog;
            PART_DataGrid = GetTemplateChild(nameof(PART_DataGrid)) as DataGrid;
            PART_BtnConnect = GetTemplateChild(nameof(PART_BtnConnect)) as Button;
            PART_BtnDisConnect = GetTemplateChild(nameof(PART_BtnDisConnect)) as Button;
            PART_BtnSaveConfig = GetTemplateChild(nameof(PART_BtnSaveConfig)) as Button;

            if (PART_BtnConnect != null)
            {
                PART_BtnConnect.Click -= BtnConnect_Click;
                PART_BtnConnect.Click += BtnConnect_Click;
            }

            if (PART_BtnDisConnect != null)
            {
                PART_BtnDisConnect.Click -= BtnDisConnect_Click;
                PART_BtnDisConnect.Click += BtnDisConnect_Click;
            }

            if (PART_BtnSaveConfig != null)
            {
                PART_BtnSaveConfig.Click -= BtnSaveConfig_Click;
                PART_BtnSaveConfig.Click += BtnSaveConfig_Click;
            }
        }

        private void BtnSaveConfig_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDisConnect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RSSerialPort_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CellValueEditChanged(string? obj)
        {

        }

        private void TemplateDownload(object? obj)
        {

        }

        private void ExportConfig(object? obj)
        {

        }

        private void ImportConfig(object? obj)
        {

        }

        private void DeleteModbusDataConfigModel(string? obj)
        {

        }

        private void AddData(object? obj)
        {

        }


    }
}
