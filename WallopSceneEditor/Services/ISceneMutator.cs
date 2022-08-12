using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    public class DirectorEventArgs : EventArgs
    {
        public List<string> Messages { get; init; }
        public string Package { get; init; }
        public string Module { get; init; }
        public string DirectorName { get; init; }
        public bool OperationFailed { get; init; }

        public string ModulePath => $"{Package}>{Module}";


        public DirectorEventArgs(string package, string module, string directorName, bool failed, params string[] messages)
        {
            Messages = new List<string>(messages);
            Package = package;
            Module = module;
            DirectorName = directorName;
            OperationFailed = failed;
        }
    }

    public class ActorEventArgs : EventArgs
    {
        public List<string> Messages { get; init; }
        public string Package { get; init; }
        public string Module { get; init; }
        public string ParentLayout { get; init; }
        public string ActorName { get; init; }
        public bool OperationFailed { get; init; }

        public string ModulePath => $"{Package}>{Module}";

        public ActorEventArgs(string package, string module, string parentLayout, string actorName, bool failed, params string[] messages)
        {
            Messages = new List<string>(messages);
            Package = package;
            Module = module;
            ParentLayout = parentLayout;
            ActorName = actorName;
            OperationFailed = failed;
        }
    }

    public delegate void LayoutAdded(string layoutName);


    public interface ISceneMutator
    {
        event EventHandler<DirectorEventArgs>? OnDirectorAdded;
        event EventHandler<ActorEventArgs>? OnActorAdded;
        event LayoutAdded? OnLayoutAdded;

        void AddDirector(string modulePath, string name);
        void AddActor(string parentLayout, string modulePath, string name);

        void RenameDirector(string directorName, string newName);
        void RenameActor(string parentLayout, string actorName, string newName);

        public void AddLayout(string name);
        public void RenameLayout(string layoutName, string newName);
    }
}
