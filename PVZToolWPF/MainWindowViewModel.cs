global using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            this.ReadPlantOverlap();
            this.ReadCheat();
        }
        public MainWindowViewModel()
        {
            for(int i = 0; i < 10; i++)
            {
                this.cardNums.Add($"卡槽{i+1}");
            }
            for(byte i = 0; i < 9; i++)
            {
                this.xAxiss.Add(i);
            }
            for(byte i = 0; i < 5; i++)
            {
                this.yAxiss.Add(i);
            }
            this.plantNums.Add("豌豆射手");
            this.plantNums.Add("向日葵");
            this.plantNums.Add("樱桃炸弹");

            this.zombieNums.Add("僵尸");
            this.zombieNums.Add("摇旗僵尸");
            this.zombieNums.Add("路障僵尸");
            this.zombieNums.Add("撑杆僵尸");
            this.zombieNums.Add("铁桶僵尸");
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
        private ObservableCollection<byte> xAxiss = [];
        [ObservableProperty]
        private byte xAxis = 0;
        [ObservableProperty]
        private ObservableCollection<byte> yAxiss = [];
        [ObservableProperty]
        private byte yAxis = 0;
        [ObservableProperty]
        private byte plantIDCall = 2;
        private nint plantCallBuffer = nint.Zero;
        [ObservableProperty]
        private string plantCallAddr = string.Empty;
        [RelayCommand]
        private void PlantCall()
        {
            if(plantCallBuffer == nint.Zero)
            {
                plantCallBuffer = Kernel32.VirtualAllocEx(hProcess, nint.Zero, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
            }
            this.PlantCallAddr = $"{plantCallBuffer:x}";
            byte[] bys = new byte[]
                {
                    0x60, //pushad
                    0x8B, 0x0D, 0xC0, 0x9E, 0x6A, 0x00, //mov ecx,[6a9ec0]
                    0x8B, 0x89, 0x68, 0x07, 0x00, 0x00, //mov ecx,[ecx+768]
                    0x6A, 0xFF, //push -1 固定-1
                    0x6A, 0x02, //push 2 植物ID
                    0xB8, 0x04, 0x00, 0x00, 0x00,//mov eax,4 Y轴 
                    0x6A, 0x06, //push 6 X轴
                    0x51, // push ecx
                    0xE8, 0x02, 0xD1, 0x8C, 0xFD, //call 0040D120
                    0x61, //popad
                    0xC3 // ret
                };
            bys[16] = this.PlantIDCall;//植物ID
            bys[23] = this.XAxis;
            bys[18] = this.YAxis;
            int callAddress = 0x40D120 - (int)plantCallBuffer - bys.Length + 2;
            byte[] b = BitConverter.GetBytes(callAddress);
            bys[bys.Length - 6] = b[0]; // call指令相对地址处理
            bys[bys.Length - 5] = b[1];
            bys[bys.Length - 4] = b[2];
            bys[bys.Length - 3] = b[3];
            MemoryUtil.WriteProcessMemoryBytes(bys, (int)plantCallBuffer);
            Kernel32.SafeHTHREAD hthread = Kernel32.CreateRemoteThread(hProcess, null, 0, plantCallBuffer, nint.Zero, 0, out _);
            Kernel32.WaitForSingleObject(hthread, Kernel32.INFINITE);
            hthread.Close();
        }
        [RelayCommand]
        private void AllPlantCall()
        {
            if (plantCallBuffer == nint.Zero)
            {
                plantCallBuffer = Kernel32.VirtualAllocEx(hProcess, nint.Zero, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
            }
            this.PlantCallAddr = $"{plantCallBuffer:x}";
            byte[] bys = new byte[]
                {
                    0x60, //pushad
                    0x8B, 0x0D, 0xC0, 0x9E, 0x6A, 0x00, //mov ecx,[6a9ec0]
                    0x8B, 0x89, 0x68, 0x07, 0x00, 0x00, //mov ecx,[ecx+768]
                    0x6A, 0xFF, //push -1 固定-1
                    0x6A, 0x02, //push 2 植物ID
                    0xB8, 0x04, 0x00, 0x00, 0x00,//mov eax,4 Y轴 
                    0x6A, 0x06, //push 6 X轴
                    0x51, // push ecx
                    0xE8, 0x02, 0xD1, 0x8C, 0xFD, //call 0040D120
                    0x61, //popad
                    0xC3 // ret
                };
            for(byte x = 0; x < 9; x++)
            {
                for(byte y = 0; y < 5; y++)
                {
                    bys[16] = this.PlantIDCall;//植物ID
                    bys[23] = x;
                    bys[18] = y;
                    int callAddress = 0x40D120 - (int)plantCallBuffer - bys.Length + 2;
                    byte[] b = BitConverter.GetBytes(callAddress);
                    bys[bys.Length - 6] = b[0]; // call指令相对地址处理
                    bys[bys.Length - 5] = b[1];
                    bys[bys.Length - 4] = b[2];
                    bys[bys.Length - 3] = b[3];
                    MemoryUtil.WriteProcessMemoryBytes(bys, (int)plantCallBuffer);
                    Kernel32.SafeHTHREAD hthread = Kernel32.CreateRemoteThread(hProcess, null, 0, plantCallBuffer, nint.Zero, 0, out _);
                    Kernel32.WaitForSingleObject(hthread, Kernel32.INFINITE);
                    hthread.Close();
                }
            }
           
        }
        #endregion
        #region 植物重叠
        [ObservableProperty]
        private bool allowPlantOverlap = false;
        private void ReadPlantOverlap()
        {
            byte[] bys = new byte[] { 0x0F, 0x84, 0x1F, 0x09, 0x00,0x00 };
            int address = 0x40FE2F;
            byte[] rs = MemoryUtil.ReadProcessMemoryBytes(address, 6);
            for(int i = 0; i < bys.Length; i++)
            {
                if (bys[i] != rs[i])
                {
                    this.AllowPlantOverlap = true;
                    break;
                }
            }
        }
        [RelayCommand]
        private void PlantOverlap()
        {
            byte[] bys = [0x0F, 0x84, 0x1F, 0x09, 0x00, 0x00];
            int address = 0x40FE2F;
            if(this.AllowPlantOverlap)
            {
                bys = [0xE9, 0x20, 0x09, 0x00, 0x00, 0x90];
            }
            MemoryUtil.WriteProcessMemoryBytes(bys, address);
        }
        #endregion
        #region 僵尸种植call
        [ObservableProperty]
        private byte zombieXAxis = 6;
        [ObservableProperty]
        private byte zombieYAxis = 3;
        [ObservableProperty]
        private byte zombieNum = 3;
        [ObservableProperty]
        private ObservableCollection<string> zombieNums = new();
        nint zombieCallBuf = nint.Zero;
        [RelayCommand]
        private void ZombieCall()
        {
            if(this.zombieCallBuf == nint.Zero)
            {
                this.zombieCallBuf = Kernel32.VirtualAllocEx(hProcess, nint.Zero, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
            }
            byte[] bys = [
                0x60, //pushad
                0x6A , 0x04 , //push 4 X轴
                0x6A , 0x03 , //push 3 僵尸ID
                0xB8 , 0x02 , 0x00 , 0x00 , 0x00 , // mov eax,2 Y轴
                0x8B , 0x0D , 0xC0 , 0x9E , 0x6A , 0x00 , //mov ecx,[6a9ec0]
                0x8B , 0x89 , 0x68 , 0x07 , 0x00 , 0x00 , // mov ecx,[ecx+768]
                0x8B , 0x89 , 0x60 , 0x01 , 0x00 , 0x00 , // mov ecx,[ecx+160]
                0xE8 , 0xCF , 0xA0 , 0xC1 , 0xFF , //cal 42a0f0
                0x61 , //popad
                0xC3   //ret
                ];
            bys[2] = this.ZombieXAxis;
            bys[4] = this.ZombieNum;
            bys[6] = this.ZombieYAxis;
            int address = 0x42a0f0 - (int)zombieCallBuf - bys.Length + 2;
            byte[] bs = BitConverter.GetBytes(address);
            bys[bys.Length - 6] = bs[0];
            bys[bys.Length - 5] = bs[1];
            bys[bys.Length - 4] = bs[2];
            bys[bys.Length - 3] = bs[3];

            MemoryUtil.WriteProcessMemoryBytes(bys, (int)zombieCallBuf);
            Kernel32.SafeHTHREAD hthread = Kernel32.CreateRemoteThread(hProcess, null, 0, zombieCallBuf, nint.Zero, 0, out _);
            Kernel32.WaitForSingleObject(hthread, Kernel32.INFINITE);
            hthread.Close();
        }
        [RelayCommand]
        private void ZombieCallY()
        {
            if (this.zombieCallBuf == nint.Zero)
            {
                this.zombieCallBuf = Kernel32.VirtualAllocEx(hProcess, nint.Zero, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
            }
            byte[] bys = [
                0x60, //pushad
                0x6A , 0x04 , //push 4 X轴
                0x6A , 0x03 , //push 3 僵尸ID
                0xB8 , 0x02 , 0x00 , 0x00 , 0x00 , // mov eax,2 Y轴
                0x8B , 0x0D , 0xC0 , 0x9E , 0x6A , 0x00 , //mov ecx,[6a9ec0]
                0x8B , 0x89 , 0x68 , 0x07 , 0x00 , 0x00 , // mov ecx,[ecx+768]
                0x8B , 0x89 , 0x60 , 0x01 , 0x00 , 0x00 , // mov ecx,[ecx+160]
                0xE8 , 0xCF , 0xA0 , 0xC1 , 0xFF , //cal 42a0f0
                0x61 , //popad
                0xC3   //ret
                ];
            bys[2] = this.ZombieXAxis;
            for (int i = 0; i < 5; i++)
            {
                bys[4] = this.ZombieNum;
                bys[6] = (byte)i;
                int address = 0x42a0f0 - (int)zombieCallBuf - bys.Length + 2;
                byte[] bs = BitConverter.GetBytes(address);
                bys[bys.Length - 6] = bs[0];
                bys[bys.Length - 5] = bs[1];
                bys[bys.Length - 4] = bs[2];
                bys[bys.Length - 3] = bs[3];

                MemoryUtil.WriteProcessMemoryBytes(bys, (int)zombieCallBuf);
                Kernel32.SafeHTHREAD hthread = Kernel32.CreateRemoteThread(hProcess, null, 0, zombieCallBuf, nint.Zero, 0, out _);
                Kernel32.WaitForSingleObject(hthread, Kernel32.INFINITE);
                hthread.Close();
            }
        }
        #endregion
        #region 紫卡无限制
        [ObservableProperty]
        private bool isCheat = false;
        private void ReadCheat()
        {
            int address = 0x6a9ec0;
            int flag = MemoryUtil.ReadProcessMemoryInt(address, 0x814);
            if(flag == 1)
            {
                this.IsCheat = true;
            }
        }
        [RelayCommand]
        private void WriteCheat()
        {
            int value = 0;
            if(this.IsCheat)
            {
                value = 1;
            }
            MemoryUtil.WriteProcessMemoryInt(value, 0x6a9ec0, 0x814);
        }
        #endregion
    }
}
