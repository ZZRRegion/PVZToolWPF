global using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using PVZToolWPF.Util;

namespace PVZToolWPF
{
    internal partial class MainWindowViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private Kernel32.SafeHPROCESS hProcess = Kernel32.SafeHPROCESS.Null;
        private int baseAddress;
        [ObservableProperty]
        private int sunValue;
        [ObservableProperty]
        private bool lockSun;
        public const string SoftName = "植物大战僵尸中文版修改器-WPF";
        [ObservableProperty]
        private string title = SoftName;
        [ObservableProperty]
        private bool isCardNoCD1 = false;
        [ObservableProperty]
        private bool isCardNoCD2 = false;
        public void Update(Kernel32.SafeHPROCESS hProcess, int baseAddress)
        {
            this.hProcess = hProcess;
            this.baseAddress = baseAddress;
            this.ReadCardNoCD1();
            this.ReadCardNoCD2();
        }
        #region 卡槽无冷却
        [ObservableProperty]
        private string cardNoCD1Memo = "修改内存地址:0x487296处的，0x147E为0x147D，即更改jle->jge";
        private void ReadCardNoCD1()
        {
            int address = this.baseAddress + 0x87296;
            short value = MemoryUtil.ReadProcessMemoryShort(address);
            if(value == 0x147D)
            {
                this.IsCardNoCD1 = true;
            }
        }
        [RelayCommand]
        private void CardNoCD1()
        {
            int address = this.baseAddress + 0x87296;
            short value = 0x147E;//旧代码指令
            if (this.IsCardNoCD1)//改为无冷却
            {
                value = 0x147D;
            }
            MemoryUtil.WriteProcessMemoryShort(value, address);
        }
        [ObservableProperty]

        private string cardNoCD2Memo = "修改内存地址:0x488E73处的 C6 45 48 00改为c6 45 48 01";
        private void ReadCardNoCD2()
        {
            int address = this.baseAddress + 0x88E73;
            int value = MemoryUtil.ReadProcessMemoryInt(address);
            if(value == 0x014845C6)
            {
                this.IsCardNoCD2 = true;
            }
        }
        [RelayCommand]
        private void CardNoCD2()
        {
            int address = this.baseAddress + 0x88E73;
            int value = 0x004845C6;//旧代码指令
            if (this.IsCardNoCD2)//改为无冷却
            {
                value = 0x014845C6;
            }
            MemoryUtil.WriteProcessMemoryInt(value, address);
        }
        #endregion
    }
}
