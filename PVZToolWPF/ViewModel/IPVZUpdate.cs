using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZToolWPF.ViewModel
{
    internal interface IPVZUpdate
    {
        void Update(Kernel32.SafeHPROCESS hProcess, int baseAddress);
    }
}
