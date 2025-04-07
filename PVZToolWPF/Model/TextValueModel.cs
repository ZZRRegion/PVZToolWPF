using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZToolWPF.Model
{
    public class TextValueModel
    {
        public TextValueModel(string text, string value)
        {
            this.Text = text;
            this.Address = Convert.ToInt32(value, 16);
        }
        public int Address { get; set; }
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
