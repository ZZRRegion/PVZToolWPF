global using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZToolWPF
{
    internal partial class MainWindowViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        [ObservableProperty]
        private int sunValue;
        [ObservableProperty]
        private bool lockSun;
        public const string SoftName = "植物大战僵尸中文版修改器-WPF";
        [ObservableProperty]
        private string title = SoftName;
    }
}
