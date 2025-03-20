using PVZToolWPF.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZToolWPF.ViewModel
{
    internal partial class CardNoCDControlViewModel : ObservableObject,IPVZUpdate
    {
        private Kernel32.SafeHPROCESS hPROCESS = Kernel32.SafeHPROCESS.Null;
        private int baseAddress = 0;
        public void Update(Kernel32.SafeHPROCESS hPROCESS, int baseAddress)
        {
            this.hPROCESS = hPROCESS;
            this.baseAddress = baseAddress;
        }
        [ObservableProperty]
        private ObservableCollection<string> cardCDs = [];
        [ObservableProperty]
        private ObservableCollection<string> sunClicks = [];
        public CardNoCDControlViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                cardCDs.Add("");
                sunClicks.Add("");
            }
        }
        public void UpdateDate()
        {
            if(this.hPROCESS.IsInvalid)
            {
                return;
            }
            int address = 0x496BC8;
            for (int i = 0; i < 10; i++)
            {
                int offset = 0x70 + i * 0x50;
                this.CardCDs[i] = MemoryUtil.ReadProcessMemoryByte(address, 0x8, 0x144, offset).ToString();
            }
            address = 0x6A9EC0;
            for(int i = 0; i < 10; i++)
            {
                int offset = 0x50 + i * 0xD8;
                this.SunClicks[i] = MemoryUtil.ReadProcessMemoryInt(address, 0x768, 0xE4, offset).ToString();
            }
        }
    }
}
