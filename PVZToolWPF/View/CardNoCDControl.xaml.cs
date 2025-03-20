using PVZToolWPF.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace PVZToolWPF.View
{
    /// <summary>
    /// CardNoCDControl.xaml 的交互逻辑
    /// </summary>
    public partial class CardNoCDControl : UserControl,IPVZUpdate
    {
        public CardNoCDControl()
        {
            InitializeComponent();
            DispatcherTimer dispatcherTime = new()
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            dispatcherTime.Tick += DispatcherTime_Tick;
            dispatcherTime.Start();
        }
        public void Update(Kernel32.SafeHPROCESS hprocess, int baseAddres)
        {
            this.viewModel.Update(hprocess, baseAddres);
        }
        private void DispatcherTime_Tick(object? sender, EventArgs e)
        {
            this.viewModel.UpdateDate();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
