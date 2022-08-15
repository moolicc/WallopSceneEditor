using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models.EventData.Mutator
{
    public class DirectorEventArgs : MutatorEventArgs
    {
        public string Package { get; init; }
        public string Module { get; init; }
        public string DirectorName { get; init; }

        public string ModulePath => $"{Package}>{Module}";


        public DirectorEventArgs(string package, string module, string directorName, bool failed, params Message[] messages)
        {
            Package = package;
            Module = module;
            DirectorName = directorName;
            HasError = failed;
            Messages = new List<Message>(messages);
        }
    }
}
