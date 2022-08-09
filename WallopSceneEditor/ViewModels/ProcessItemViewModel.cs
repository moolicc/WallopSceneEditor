using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.ViewModels
{
    public class ProcessItemViewModel
    {
        public static readonly ProcessItemViewModel Empty = new ProcessItemViewModel("None", -1);

        public string Name { get; init; }
        public int ProcessId { get; init; }
        public string Text => $"{Name} ({ProcessId})";

        public ProcessItemViewModel(string name, int processId)
        {
            Name = name;
            ProcessId = processId;
        }
    }
}
