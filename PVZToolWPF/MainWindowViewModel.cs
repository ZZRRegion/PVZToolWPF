global using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace PVZToolWPF
{
    internal partial class MainWindowViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private Kernel32.SafeHPROCESS hProcess;
        private int baseAddress;
        [ObservableProperty]
        private int sunValue;
        [ObservableProperty]
        private bool lockSun;
        public const string SoftName = "植物大战僵尸中文版修改器-WPF";
        [ObservableProperty]
        private string title = SoftName;
        [ObservableProperty]
        private bool isCardNoCD = false;
        public void Update(Kernel32.SafeHPROCESS hProcess, int baseAddress)
        {
            this.hProcess = hProcess;
            this.baseAddress = baseAddress;
        }
        [ObservableProperty]
        private string cardNoCD1Memo = "修改内存地址:0x487296处的，0x147E为0x147D";
        [RelayCommand]
        private void CardNoCD1()
        {
            int address = this.baseAddress + 0x87296;
            Kernel32.MEM_PROTECTION oldProtection = Kernel32.MEM_PROTECTION.PAGE_NOCACHE;
            Kernel32.VirtualProtectEx(hProcess, address, 1024, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE, out oldProtection);
            short value = 0x147E;//旧代码指令
            if (this.IsCardNoCD)//改为无冷却
            {
                value = 0x147D;
            }
            nint buf = Marshal.AllocCoTaskMem(2);
            Marshal.WriteInt16(buf, value);
            Kernel32.WriteProcessMemory(hProcess, address, buf, 2, out _);
            Kernel32.VirtualProtectEx(hProcess, address, 2, oldProtection, out _);
            Marshal.FreeCoTaskMem(buf);
        }
    }
}
