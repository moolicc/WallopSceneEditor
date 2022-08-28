using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.ViewModels
{
    public class EngineItemViewModel
    {
        public string Text { get; set; }
        public int? ProcessId { get; set; }

        public EngineItemViewModel(string text, int? processId)
        {
            Text = text;
            ProcessId = processId;
        }
    }
}
