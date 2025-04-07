using CommunityToolkit.Mvvm.Messaging;
using PVZToolWPF.Model;
using PVZToolWPF.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZToolWPF.ViewModel
{
    
    internal partial class PlantPowerControlViewModel : ObservableRecipient, IRecipient<UpdateModel>
    {
        [ObservableProperty]
        private ObservableCollection<TextValueModel> _textValues = new();
        [ObservableProperty]
        private ObservableCollection<TextValueModel> _bloods = new();
        private Kernel32.SafeHPROCESS? hProcess;
        private int baseAddress;
        public PlantPowerControlViewModel()
        {
            string txt = @"69F1C8\\普通豌豆攻击力
                            69F1D4\\冰豌豆攻击力
                            69F1E0\\卷心菜攻击力
                            69F1EC\\普通西瓜攻击力
                            69F1F8\\孢子攻击力
                            69F204\\冰西瓜攻击力
                            69F210\\火豌豆攻击力
                            69F21C\\星星攻击力
                            69F228\\尖刺攻击力
                            69F234\\篮球攻击力
                            69F240\\玉米粒攻击力
                            69F258\\黄油攻击力
                            69F264\\豌豆僵尸攻击力";
            foreach(var item in txt.Split("\r\n"))
            {
                string[] str = item.Trim().Split("\\\\");
                this._textValues.Add(new TextValueModel(str[1], str[0]));
            }
            txt = @"45DC55\\一般植物的血量
                    45E1A7\\坚果血量
                    45E215\\高坚果血量
                    45E445\\南瓜头血量
                    45E242\\大蒜血量
                    45E5C3\\地刺王血量
                    5227BB\\一般僵尸血量
                    522892\\路障饰品血量
                    522CBF\\撑杆僵尸血量
                    52292B\\铁桶饰品血量
                    52337D\\报纸饰品血量
                    522949\\铁门饰品血量
                    522BB0\\橄榄球饰品血量
                    523530\\舞王僵尸血量
                    522DE1\\冰车僵尸血量
                    523139\\雪橇车饰品血量
                    522D64\\海豚僵尸血量
                    522FC7\\小丑僵尸血量
                    522BEF\\矿工僵尸血量
                    523300\\跳跳僵尸血量
                    52296E\\雪人僵尸血量
                    522A1B\\蹦极僵尸血量
                    52299C\\梯子僵尸血量
                    522E8D\\投石车僵尸血量
                    523D26\\巨人僵尸血量
                    523624\\僵尸博士血量
                    52361E\\僵尸博士在小游戏里增加的血量
                    52382B\\坚果饰品血量
                    523A87\\辣椒尸血量
                    52395D\\高坚果饰品血量
                    523E4A\\红眼僵尸血量
                    5235AC\\小鬼僵尸在ize的血量
                    5234BF\\气球僵尸的气球血量";
            foreach (var item in txt.Split("\r\n"))
            {
                string[] str = item.Trim().Split("\\\\");
                this._bloods.Add(new TextValueModel(str[1], str[0]));
            }
            this.Messenger.RegisterAll(this, PVZMsgToken.Update);
        }

        public void Receive(UpdateModel message)
        {
            this.hProcess = message.SafeHPROCESS;
            this.baseAddress = message.BaseAddress;
            this.Update();
        }
        private void Update()
        {
            foreach(var item in this.TextValues)
            {
                item.Value = MemoryUtil.ReadProcessMemoryInt(item.Address);
            }
            foreach(var item in this.Bloods)
            {
                item.Value = MemoryUtil.ReadProcessMemoryInt(item.Address);
            }
        }
    }
}
