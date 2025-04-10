﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara;
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
        public static byte[] ReadProcessMemoryBytes(int baseAddr, int count)
        {
            byte[] bys = new byte[count];
            nint buf = Marshal.AllocCoTaskMem(count);
            if(Kernel32.ReadProcessMemory(HProcess, baseAddr, buf, count, out _))
            {
                for (int i = 0; i < count; i++)
                {
                    bys[i] = Marshal.ReadByte(buf + i);
                }
            }
            Marshal.FreeCoTaskMem(buf);
            return bys;
        }
        public static int ReadProcessMemoryInt(int baseAddr, int one)
        {
            baseAddr = ReadProcessMemoryInt(baseAddr) + one;
            return ReadProcessMemoryInt(baseAddr);
        }
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
        public static byte ReadProcessMemoryByte(int baseAddr)
        {
            byte value = 0;
            nint buf = Marshal.AllocCoTaskMem(1);
            if(Kernel32.ReadProcessMemory(HProcess, baseAddr, buf, 1, out _))
            {
                value = Marshal.ReadByte(buf);
            }

            return value;
        }
        public static byte ReadProcessMemoryByte(int baseAddr, int one, int two, int three)
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
        public static bool WriteProcessMemoryBytes(byte[] bys, int baseAddr)
        {
            //修改目标内存地址权限
            Kernel32.VirtualProtectEx(HProcess, baseAddr, bys.Length, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE, out var old);
            bool flag = Kernel32.WriteProcessMemory(HProcess, baseAddr, bys, bys.Length, out _);
            Kernel32.VirtualProtectEx(HProcess, baseAddr, bys.Length, old, out old);
            return flag;
        }
        public static bool WriteProcessMemoryBytes(byte[] bys, nint baseAddr)
        {
            //修改目标内存地址权限
            return WriteProcessMemoryBytes(bys, (int)baseAddr);
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
        public static bool WriteProcessMemoryInt(int value, int baseAddr, int one)
        {
            bool flag = false;
            nint buf = Marshal.AllocCoTaskMem(4);
            int address = baseAddr;
            if (Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
            {
                address = Marshal.ReadInt32(buf) + one;
                Marshal.WriteInt32(buf, value);
                flag = Kernel32.WriteProcessMemory(HProcess, address, buf, 4, out _);
            }
            Marshal.FreeCoTaskMem(buf);
            return flag;
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
        public static bool WriteProcessMemoryInt(int value, int baseAddr, int one, int two, int three)
        {
            bool flag = false;
            nint buf = Marshal.AllocCoTaskMem(4);
            int address = baseAddr;
            if (Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
            {
                address = Marshal.ReadInt32(buf) + one;
                if (Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
                {
                    address = Marshal.ReadInt32(buf) + two;
                    if (Kernel32.ReadProcessMemory(HProcess, address, buf, 4, out _))
                    {
                        address = Marshal.ReadInt32(buf) + three;
                        flag = WriteProcessMemoryInt(value, address);
                    }
                }
            }
            Marshal.FreeCoTaskMem(buf);
            return flag;
        }
        /// <summary>
        /// 写浮点型数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseAddr"></param>
        /// <returns></returns>
        public static bool WriteProcessMemoryFloat(float value, int baseAddr)
        {
            Kernel32.MEM_PROTECTION oldProtection = Kernel32.MEM_PROTECTION.PAGE_NOCACHE;
            Kernel32.VirtualProtectEx(HProcess, baseAddr, 1024, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE, out oldProtection);
            nint buf = Marshal.AllocCoTaskMem(4);
            Marshal.Copy(BitConverter.GetBytes(value), 0, buf, 4);
            Kernel32.WriteProcessMemory(HProcess, baseAddr, buf, 4, out _);
            Kernel32.VirtualProtectEx(HProcess, baseAddr, 1024, oldProtection, out _);
            Marshal.FreeCoTaskMem(buf);
            return true;
        }
        /// <summary>
        /// 申请内存
        /// </summary>
        /// <returns></returns>
        public static nint VirtualAllocEx()
        {
            nint buf = Kernel32.VirtualAllocEx(HProcess, nint.Zero, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
            return buf;
        }
        public static nint VirtualAllocEx(nint lpAddress)
        {
            nint buf = Kernel32.VirtualAllocEx(HProcess, lpAddress, 1024, Kernel32.MEM_ALLOCATION_TYPE.MEM_COMMIT, Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE);
            return buf;
        }
        /// <summary>
        /// 创建远程线程运行
        /// </summary>
        /// <param name="threadAddr"></param>
        /// <returns></returns>
        public static void CreateRemoteThread(nint threadAddr)
        {
            Kernel32.SafeHTHREAD hthread = Kernel32.CreateRemoteThread(HProcess, null, 0, threadAddr, nint.Zero, 0, out _);
            Kernel32.WaitForSingleObject(hthread, Kernel32.INFINITE);
            hthread.Close();
        }
    }
}
