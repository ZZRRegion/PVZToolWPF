global using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using PVZToolWPF.Util;
using PVZToolWPF.ViewModel;
using Vanara.PInvoke;

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
        private string PVZTitle = "植物大战僵尸中文版";
        private uint pid;
        public event Action<Kernel32.SafeHPROCESS, int, uint>? UpdateEvent;
        [RelayCommand]
        private void Reload()
        {
            string errMsg = string.Empty;
            HWND hwnd = User32.FindWindow(null, PVZTitle);
            if (hwnd != HWND.NULL)
            {
                uint tid = User32.GetWindowThreadProcessId(hwnd, out pid);
                if (tid > 0)
                {
                    hProcess = Kernel32.OpenProcess(ACCESS_MASK.GENERIC_ALL, false, pid);
                    MemoryUtil.HProcess = hProcess;
                    HINSTANCE[] hinstances = Kernel32.EnumProcessModules(hProcess);
                    this.baseAddress = hinstances[0].DangerousGetHandle().ToInt32();
                    this.UpdateEvent?.Invoke(hProcess, baseAddress,pid);
                    this.Update(hProcess, baseAddress);
                }
            }
        }
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
            ReadPurpleCardUnlimited();
            ReadPlantPurpleCard();
            this.SeckillHook();
            ReadBackgroundRun();
            this.ReadRandBoom();
            this.ReadPot();
            this.ReadConveyorDelay();
            this.ReadVerticalPlant();
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
            DispatcherTimer dispatcherTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(200),
            };
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if(!this.hProcess.IsInvalid && this.IsRandBoom)
            {
                int address = 0x6bbf20; //这是子弹的赋值地址
                int value = Random.Shared.Next(0, 13);
                MemoryUtil.WriteProcessMemoryInt(value, address);
            }
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
            bys[16] = 25;// this.PlantIDCall;//植物ID
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
            for (byte j = 4; j <= 9; j++)
            {
                bys[2] = j;
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
        }
        #endregion
        #region 紫卡无限制
        /// <summary>
        /// 坐标标志
        /// </summary>
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

        /// <summary>
        /// 紫卡无限制
        /// </summary>
        [ObservableProperty]
        private bool isPurpleCardUnlimited = false;
        private void ReadPurpleCardUnlimited()
        {
            int address = 0x41d7d0;
            byte value = MemoryUtil.ReadProcessMemoryByte(address);
            if(value != 0x51)
            {
                this.IsPurpleCardUnlimited = true;
            }
        }
        [RelayCommand]
        private void WritePurpleCardUnlimited()
        {
            int address = 0x41d7d0;
            byte value = 0x51;
            if(this.IsPurpleCardUnlimited)
            {
                value = 0xC3;
            }
            MemoryUtil.WriteProcessMemoryBytes([value], address);
        }

        /// <summary>
        /// 紫卡直接种植
        /// </summary>
        [ObservableProperty]
        private bool isPlantPurpleCard = false;
        private void ReadPlantPurpleCard()
        {
            int address = 0x40E477;
            short value = MemoryUtil.ReadProcessMemoryShort(address);
            if(value != 0x4674)
            {
                this.IsPlantPurpleCard = true;
            }
        }
        [RelayCommand]
        private void WritePlantPurpleCard()
        {
            int address = 0x40E477;
            short value = 0x4674;
            if(this.IsPlantPurpleCard)
            {
                value = 0x46EB;
            }
            MemoryUtil.WriteProcessMemoryShort(value, address);
        }
        #endregion
        #region 全屏秒杀
        [RelayCommand]
        private void Seckill()
        {
            int address = 0x722000;
            for(int i = 0; i < 500; i++)
            {
                MemoryUtil.WriteProcessMemoryInt(3, address, i*348 + 0x28);
                MemoryUtil.WriteProcessMemoryInt(3, address, -i * 348 + 0x28);
            }
        }
        /// <summary>
        /// 全屏秒杀hook,需要先执行
        /// </summary>
        private void SeckillHook()
        {
            byte[] bys = [
                0xE9, 0x24, 0x46, 0x1F, 0x00, //jmp 72200a
                0x90, //nop
                0x90 //nop
                ];
            int address = 0x52D9E1;
            MemoryUtil.WriteProcessMemoryBytes(bys, address);
            address = 0x72200A;
            byte[] jmps = [
                0x60, //pushad
                0x8D, 0x01, //lea eax,[ecx]
                0xA3, 0x00, 0x20, 0x72, 0x00, //mov [722000],eax
                0x61, //popad
                0xD9, 0x41, 0x2C, //fld dword ptr [ecx+24]
                0x57, //push edi
                0xDA, 0x61, 0x08, //fisub [ecx+08]
                0xE9, 0xC9, 0xB9, 0xE0, 0xFF //jmp 52d9e8
                ];
            MemoryUtil.WriteProcessMemoryBytes(jmps, address);
        }
        #endregion
        #region 后台运行
        [ObservableProperty]
        private bool isBackgroundRun = false;
        private void ReadBackgroundRun()
        {
            int address = 0x546310;
            byte[] bys = [0x8B, 0x81, 0x2C, 0x03, 0x00, 0x00];
            byte[] bs = MemoryUtil.ReadProcessMemoryBytes(address, 6);
            for(int i = 0; i < bys.Length; i++)
            {
                if (bys[i] != bs[i])
                {
                    this.IsBackgroundRun = true;
                    break;
                }
            }
        }
        [RelayCommand]
        private void WriteBackgroundRun()
        {
            byte[] bys = [0x8B, 0x81, 0x2C, 0x03, 0x00, 0x00];
            if(this.IsBackgroundRun)
            {
                bys = [0x90, 0x90, 0x90, 0x90, 0x90, 0x90];
            }
            int address = 0x546310;

            MemoryUtil.WriteProcessMemoryBytes(bys, address);
        }
        #endregion
        #region 随机子弹
        [ObservableProperty]
        private bool isRandBoom = false;
        private void ReadRandBoom()
        {
            byte[] bys = [0x89, 0x45, 0x5C, 0x8B, 0xC6];
            int address = 0x46c769;
            byte[] bs = MemoryUtil.ReadProcessMemoryBytes(address, 5);
            for(int i = 0; i < bys.Length; i++)
            {
                if (bys[i] != bs[i])
                {
                    this.IsRandBoom = true;
                    break;
                }
            }

        }
        private nint randBoomBuf = nint.Zero;
        [RelayCommand]
        private void WriteRandBoom()
        {
            byte[] bys = [0x89, 0x45, 0x5C, 0x8B, 0xC6];//原指令
            int address = 0x46c769;
            if(randBoomBuf == nint.Zero)
            {
                randBoomBuf = Kernel32.VirtualAllocEx(hProcess, nint.Zero, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
            }
            if(this.IsRandBoom)
            {
                byte[] bs = [0xE9, 0x92, 0x38, 0xF8, 0x01]; //jmp randBoomBuf
                int jmpaddr = (int)randBoomBuf - 0x46c76e;
                byte[] ts = BitConverter.GetBytes(jmpaddr);
                bs[1] = ts[0];
                bs[2] = ts[1];
                bs[3] = ts[2];
                bs[4] = ts[3];
                MemoryUtil.WriteProcessMemoryBytes(bs, address);

                byte[] threadBuf = [
                    0x8B, 0x05, 0x20, 0xBF, 0x6B, 0x00, //mov eax,[6bbf20]
                    0x89, 0x45, 0x5C, //mov [ebp+5c],eax
                    0x8B, 0xC6, //mov eax,esi
                    0xE9, 0x5E, 0xC7, 0x07, 0xFE //jmp 46c76e
                    ];
                jmpaddr = 0x46c76E -(int)randBoomBuf - threadBuf.Length;
                ts = BitConverter.GetBytes(jmpaddr);
                threadBuf[threadBuf.Length - 4] = ts[0];
                threadBuf[threadBuf.Length - 3] = ts[1];
                threadBuf[threadBuf.Length - 2] = ts[2];
                threadBuf[threadBuf.Length - 1] = ts[3];
                MemoryUtil.WriteProcessMemoryBytes(threadBuf, (int)randBoomBuf);
            }
            else
            {
                MemoryUtil.WriteProcessMemoryBytes(bys, address);
            }
        }
        #endregion
        #region 陶罐透视
        [ObservableProperty]
        private bool isPot = false;
        private void ReadPot()
        {
            int address = 0x44DBF4;
            byte[] bys = [0x83, 0x7D, 0x4C, 0x00, 0x0F, 0x8E, 0x18, 0x03, 0x00, 0x00];
            byte[] bs = MemoryUtil.ReadProcessMemoryBytes(address, bys.Length);
            for(int i = 0; i < bys.Length; i++)
            {
                if (bys[i] != bs[i])
                {
                    this.IsPot = true;
                    break;
                }
            }
        }
        private nint potThreadBuf = nint.Zero;
        [RelayCommand]
        private void WritePot()
        {
            int address = 0x44DBF4;
            byte[] bys = [0x83, 0x7D, 0x4C, 0x00, 0x0F, 0x8E, 0x18, 0x03, 0x00, 0x00];
            if(this.IsPot)
            {
                if(potThreadBuf == nint.Zero)
                {
                    potThreadBuf = Kernel32.VirtualAllocEx(hProcess, nint.Zero, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
                }
                byte[] jmpBys = [
                    0xE9, 0x07, 0x24, 0x40, 0x00, //jmp potThreadBuf
                    0x0F, 0x1F, 0x44, 0x00, 0x00//nop
                    ];
                int addr = (int)potThreadBuf - 0x44DBF4 - 5;
                byte[] ts = BitConverter.GetBytes(addr);
                jmpBys[1] = ts[0];
                jmpBys[2] = ts[1];
                jmpBys[3] = ts[2];
                jmpBys[4] = ts[3];
                MemoryUtil.WriteProcessMemoryBytes(jmpBys, address);

                byte[] buf = [
                    0xC7, 0x45, 0x4C, 0x32, 0x00, 0x00, 0x00, //[ebp+4C],50
                    0x83, 0x7D, 0x4C, 0x00, //com dword ptr[ebp+4C],00
                    0x0F, 0x8E, 0x05, 0xDF, 0xBF, 0xFF, //jng 44DF16
                    0xE9, 0xE8, 0xDB, 0xBF, 0xFF //jmp 44DBFE
                    ];
                addr = 0x44DBFE - (int)potThreadBuf - buf.Length;
                ts = BitConverter.GetBytes(addr);
                buf[buf.Length - 4] = ts[0];
                buf[buf.Length - 3] = ts[1];
                buf[buf.Length - 2] = ts[2];
                buf[buf.Length - 1] = ts[3];
                MemoryUtil.WriteProcessMemoryBytes(buf, (int)potThreadBuf);

            }
            else
            {
                MemoryUtil.WriteProcessMemoryBytes(bys, address);
            }
        }
        #endregion
        #region 传送带无延迟
        [ObservableProperty]
        private bool isConveyorDelay = false;
        private void ReadConveyorDelay()
        {
            int address = 0x422D17;
            byte[] bys = [0x83, 0x43, 0x5C, 0xFF];
            byte[] bs = MemoryUtil.ReadProcessMemoryBytes(address, bys.Length);
            for(int i = 0; i < bys.Length; i++)
            {
                if (bys[i] != bs[i])
                {
                    this.IsConveyorDelay = true;
                    break;
                }
            }
        }
        [RelayCommand]
        private void WriteConveyorDelay()
        {
            int address = 0x422D17;
            byte[] bys = [0x83, 0x43, 0x5C, 0xFF];
            if(this.IsConveyorDelay)
            {
                bys = [0x83, 0x43, 0x5C, 0x80];
            }
            MemoryUtil.WriteProcessMemoryBytes(bys, address);
            address = 0x489CA1;
            bys = [0x85, 0xC0];
            if(this.IsConveyorDelay)
            {
                bys = [0x31, 0xC0];
            }
            MemoryUtil.WriteProcessMemoryBytes(bys, address);
        }
        #endregion
        #region 竖排种植
        [ObservableProperty]
        private bool isVerticalPlant = false;
        private void ReadVerticalPlant()
        {
            int address = 0x410AE6;
            byte[] bys = [0x0F, 0x85, 0xE5, 0x00, 0x00, 0x00];
            byte[] bs = MemoryUtil.ReadProcessMemoryBytes(address, bys.Length);
            for(int i = 0; i < bys.Length; i++)
            {
                if (bys[i] != bs[i])
                {
                    this.IsVerticalPlant = true;
                    break;
                }
            }
        }
        [RelayCommand]
        private void WriteVerticalPlant()
        {
            int address = 0x410AE6;
            byte[] bys = [0x0F, 0x85, 0xE5, 0x00, 0x00, 0x00];
            if(this.IsVerticalPlant)
            {
                bys = [0x90, 0x90, 0x90, 0x90, 0x90, 0x90];
            }
            MemoryUtil.WriteProcessMemoryBytes(bys, address);
        }
        #endregion
    }
}
