using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZToolWPF.Model
{
    internal record UpdateModel(Kernel32.SafeHPROCESS SafeHPROCESS, int BaseAddress, HWND Hwnd);
    internal record ShowModel(bool Show);
    public static class PVZMsgToken
    {
        public static string Update = "Update";
        public static string Show = "Show";
    }
}
