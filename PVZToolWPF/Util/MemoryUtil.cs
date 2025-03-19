using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Vanara.PInvoke.User32;

namespace PVZToolWPF.Util
{
    /// <summary>
    /// 内存读写
    /// </summary>
    internal static class MemoryUtil
    {
        public static Kernel32.SafeHPROCESS HProcess { get; set; }
        public static int ReadProcessMemoryInt(int baseAddr, int one, int two)
        {
            int value = 0;
            int address = baseAddr;
            nint buf = Marshal.AllocCoTaskMem(4);
            if (Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
            {
                address = Marshal.ReadInt32(buf) + one;
                if (Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
                {
                    address = Marshal.ReadInt32(buf) + two;
                    if (Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
                    {
                        value = Marshal.ReadInt32(buf);
                    }
                }
            }
            Marshal.FreeCoTaskMem(buf);
            return value;
        }
        public static bool WriteProcessMemoryInt(int value, int baseAddr, int one, int two)
        {
            bool flag = false;
            nint buf = Marshal.AllocCoTaskMem(4);
            int address = baseAddr;
            if(Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
            {
                address = Marshal.ReadInt32(buf) + one;
                if(Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
                {
                    address = Marshal.ReadInt32(buf) + two;
                    Marshal.WriteInt32(buf, value);
                    flag = Kernel32.WriteProcessMemory(HProcess, address, buf, 4, out _);
                }
            }
            Marshal.FreeCoTaskMem(buf);
            return flag;
        }
    }
}
