global using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using PVZToolWPF.Util;
using PVZToolWPF.ViewModel;

namespace PVZToolWPF
{
    internal partial class MainWindowViewModel : ObservableObject,IPVZUpdate
    {
        private Kernel32.SafeHPROCESS hProcess = Kernel32.SafeHPROCESS.Null;
        private int baseAddress;
        [ObservableProperty]
        private int sunValue;
        [ObservableProperty]
        private bool lockSun;
        public const string SoftName = "植物大战僵尸中文版修改器WPF版@stdio";
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
            this.ReadAutoCollect();
            this.ReadAutoCollect2();
            this.ReadBulletStacking();
        }
        public MainWindowViewModel()
        {
            for(int i = 0; i < 10; i++)
            {
                this.cardNums.Add($"卡槽{i+1}");
            }
            this.plantNums.Add("豌豆射手");
            this.plantNums.Add("向日葵");
            this.plantNums.Add("樱桃炸弹");
        }
        [ObservableProperty]
        private bool isAutoCollect = false;
        [ObservableProperty]
        private bool isAutoCollect2 = false;
        private void ReadAutoCollect()
        {
            int address = 0x0043158F;
            short value = MemoryUtil.ReadProcessMemoryShort(address);
            if(value == 0x0874)
            {
                this.IsAutoCollect = true;
            }
        }
        private void ReadAutoCollect2()
        {
            int address = 0x00430AD0;
            short value = MemoryUtil.ReadProcessMemoryShort(address);
            if((ushort)value != 0x3E75)
            {
                this.IsAutoCollect2 = true;
            }
        }
        [RelayCommand]
        private void AutoCollect()
        {
            int address = 0x0043158F;
            short value = 0x0875;//原指令
            if(this.IsAutoCollect)
            {
                value = 0x0874;//je
            }
            MemoryUtil.WriteProcessMemoryShort(value, address);
        }
        [RelayCommand]
        private void AutoCollect2()
        {
            int address = 0x00430AD0;
            ushort value = 0x3E75;
            if(this.IsAutoCollect2)
            {
                value = 0x9090;
            }
            MemoryUtil.WriteProcessMemoryShort((short)value, address);
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
        #region 子弹叠加
        [ObservableProperty]
        private bool isBulletStacking = false;
        private void ReadBulletStacking()
        {
            int address = 0x464A96;
            byte[] oldbys = { 0x0F, 0x85, 0x98, 0xFE, 0xFF, 0xFF };
            byte[] bys = MemoryUtil.ReadProcessMemoryBytes(address, 6);
            for(int i = 0; i < 6; i++)
            {
                if (bys[i] != oldbys[i])
                {
                    this.IsBulletStacking = true;
                    break;
                }
            }
        }
        [RelayCommand]
        private void BulletStacking()
        {
            int address = 0x464A96;
            byte[] bys = { 0x0F, 0x85, 0x98, 0xFE, 0xFF, 0xFF };
            if(this.IsBulletStacking)
            {
                for(int i = 0; i < 6; i++)
                {
                    bys[i] = 0x90;
                }
            }
            MemoryUtil.WriteProcessMemoryBytes(bys, address);
        }
        #endregion
        #region 更改卡槽植物
        [ObservableProperty]
        private int cardIndex = 0;
        [ObservableProperty]
        private ObservableCollection<string> cardNums = [];
        [ObservableProperty]
        private int plantIndex = 0;
        [ObservableProperty]
        private ObservableCollection<string> plantNums = [];
        [RelayCommand]
        private void ChangedCardPlant()
        {
            int address = 0x6A9EC0;
            int offset = 0x5c + this.CardIndex * 0x50;
            MemoryUtil.WriteProcessMemoryInt(this.PlantIndex, address, 0x768, 0x144, offset);
        }
        #endregion
        #region 种植Call
        [ObservableProperty]
        private ObservableCollection<int> xAxiss = [];
        [ObservableProperty]
        private int xAxis = 0;
        [ObservableProperty]
        private ObservableCollection<int> yAxiss = [];
        [ObservableProperty]
        private int yAxis = 0;
        [ObservableProperty]
        private int plantIDCall = 0;
        private nint plantCallBuffer = nint.Zero;
        [RelayCommand]
        private void PlantCall()
        {
            if(plantCallBuffer == nint.Zero)
            {
                plantCallBuffer = Kernel32.VirtualAllocEx(hProcess, nint.Zero, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
            }
            
            Kernel32.SafeHTHREAD hthread = Kernel32.CreateRemoteThread(hProcess, null, 0, plantCallBuffer, nint.Zero, 0, out _);
            Kernel32.WaitForSingleObject(hthread, Kernel32.INFINITE);
            hthread.Close();
        }
        #endregion
    }
}
