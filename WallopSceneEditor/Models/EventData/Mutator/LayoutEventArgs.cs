using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models.EventData.Mutator
{
    public class LayoutEventArgs : MutatorEventArgs
    {
        public string LayoutName { get; set; }

        public LayoutEventArgs(string layout)
        {
            LayoutName = layout;
        }
    }
}
