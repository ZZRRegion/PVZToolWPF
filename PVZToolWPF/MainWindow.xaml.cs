global using Vanara.PInvoke;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PVZToolWPF.Util;

namespace PVZToolWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PVZTitle = "植物大战僵尸中文版";
        /// <summary>
        /// PVZ进程模块基址
        /// </summary>
        private int baseAddress = 0;
        private uint pid;
        private Kernel32.SafeHPROCESS hprocess = Kernel32.SafeHPROCESS.Null;
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer dispatcherTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(500),
            };
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if(this.pid > 0 && !this.hprocess.IsInvalid)
            {
                if(this.viewModel.LockSun)
                {
                    MemoryUtil.WriteProcessMemoryInt(this.viewModel.SunValue, this.baseAddress + 0x2A9EC0, 0x768, 0x5560);
                }
                else
                {
                    this.ReadSunValue();
                }
            }
        }
        private void ReadSunValue()
        {
            this.viewModel.SunValue = MemoryUtil.ReadProcessMemoryInt(this.baseAddress + 0x2A9EC0, 0x768, 0x5560);
            
        }

        private bool Init()
        {
            string errMsg = string.Empty;
            HWND hwnd = User32.FindWindow(null, PVZTitle);
            if(hwnd != HWND.NULL)
            {
                uint tid = User32.GetWindowThreadProcessId(hwnd, out pid);
                if(tid > 0)
                {
                    hprocess = Kernel32.OpenProcess(ACCESS_MASK.GENERIC_ALL, false, pid);
                    MemoryUtil.HProcess = hprocess;
                    HINSTANCE[] hinstances = Kernel32.EnumProcessModules(hprocess);
                    this.baseAddress = hinstances[0].DangerousGetHandle().ToInt32();
                }
            }
            return true;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Init())
            {
                this.viewModel.Title = $"{MainWindowViewModel.SoftName}-{pid:X}";
            }
        }
    }
}