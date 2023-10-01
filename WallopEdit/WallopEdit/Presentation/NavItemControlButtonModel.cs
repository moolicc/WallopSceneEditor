using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.Presentation
{
    public partial record NavItemControlButtonModel
    {
        public string Text { get; set; }
        public string IconGlyph { get; set; }
        public string Tooltip { get; set; }

        public NavItemControlButtonModel()
        {
            Text = "";
            IconGlyph = "";
            Tooltip = "";
        }
    }
}
