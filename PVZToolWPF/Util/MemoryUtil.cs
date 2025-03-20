using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace PVZToolWPF.Util
{
    /// <summary>
    /// 内存读写
    /// </summary>
    internal static class MemoryUtil
    {
        public static Kernel32.SafeHPROCESS HProcess { get; set; } = Kernel32.SafeHPROCESS.Null;
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
        public static int ReadProcessMemoryInt(int baseAddr, int one, int two,int three)
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
                        address = Marshal.ReadInt32(buf) + three;
                        if(Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
                        {
                            value = Marshal.ReadInt32(buf);
                        }
                    }
                }
            }
            Marshal.FreeCoTaskMem(buf);
            return value;
        }
        public static int ReadProcessMemoryByte(int baseAddr, int one, int two, int three)
        {
            byte value = 0;
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
                        address = Marshal.ReadInt32(buf) + three;
                        if (Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
                        {
                            value = Marshal.ReadByte(buf);
                        }
                    }
                }
            }
            Marshal.FreeCoTaskMem(buf);
            return value;
        }
        public static short ReadProcessMemoryShort(int baseAddr)
        {
            nint buf = Marshal.AllocCoTaskMem(2);
            Kernel32.ReadProcessMemory(HProcess, baseAddr, buf, 2, out _);
            short value = Marshal.ReadInt16(buf);
            Marshal.FreeCoTaskMem(buf);
            return value;
        }
        public static int ReadProcessMemoryInt(int baseAddr)
        {
            nint buf = Marshal.AllocCoTaskMem(4);
            Kernel32.ReadProcessMemory(HProcess, baseAddr, buf, 4, out _);
            int value = Marshal.ReadInt32(buf);
            Marshal.FreeCoTaskMem(buf);
            return value;
        }
        public static bool WriteProcessMemoryShort(short value, int baseAddr)
        {
            Kernel32.MEM_PROTECTION oldProtection = Kernel32.MEM_PROTECTION.PAGE_NOCACHE;
            Kernel32.VirtualProtectEx(HProcess, baseAddr, 1024, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE, out oldProtection);
            nint buf = Marshal.AllocCoTaskMem(2);
            Marshal.WriteInt16(buf, value);
            Kernel32.WriteProcessMemory(HProcess, baseAddr, buf, 2, out _);
            Kernel32.VirtualProtectEx(HProcess, baseAddr, 1024, oldProtection, out _);
            Marshal.FreeCoTaskMem(buf);
            return true;
        }
        
        public static bool WriteProcessMemoryInt(int value, int baseAddr)
        {
            Kernel32.MEM_PROTECTION oldProtection = Kernel32.MEM_PROTECTION.PAGE_NOCACHE;
            Kernel32.VirtualProtectEx(HProcess, baseAddr, 1024, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE, out oldProtection);
            nint buf = Marshal.AllocCoTaskMem(4);
            Marshal.WriteInt32(buf, value);
            Kernel32.WriteProcessMemory(HProcess, baseAddr, buf, 4, out _);
            Kernel32.VirtualProtectEx(HProcess, baseAddr, 1024, oldProtection, out _);
            Marshal.FreeCoTaskMem(buf);
            return true;
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
