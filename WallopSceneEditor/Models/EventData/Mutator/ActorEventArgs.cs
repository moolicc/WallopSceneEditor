using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models.EventData.Mutator
{
    public class ActorEventArgs : MutatorEventArgs
    {
        public string PackagePath { get; init; }
        public string Module { get; init; }
        public string ParentLayout { get; init; }
        public string ActorName { get; init; }

        public string ModulePath => $"{PackagePath}>{Module}";

        public ActorEventArgs(string package, string module, string parentLayout, string actorName, bool failed, params Message[] messages)
        {
            PackagePath = package;
            Module = module;
            ParentLayout = parentLayout;
            ActorName = actorName;
            HasError = failed;
            Messages = new List<Message>(messages);
        }
    }
}
