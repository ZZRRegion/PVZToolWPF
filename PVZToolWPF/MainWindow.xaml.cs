global using Vanara.PInvoke;
using System.Diagnostics;
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
using PVZToolWPF.ViewModel;
using Vanara.Extensions.Reflection;

namespace PVZToolWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// PVZ进程模块基址
        /// </summary>
        private int baseAddress = 0;
        private uint pid;
        private Kernel32.SafeHPROCESS hprocess = Kernel32.SafeHPROCESS.Null;
        public MainWindow()
        {
            InitializeComponent();
            this.viewModel.UpdateEvent += (hProcess, baseAddress, pid) => {
                this.hprocess = hProcess;
                this.baseAddress = baseAddress;
                this.pid = pid;
                this.viewModel.Title = $"{MainWindowViewModel.SoftName}-{pid:X}";
            };
            this.viewModel.ReloadCommand.Execute(null);
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
                    this.viewModel.SunValue = 1024;
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}