using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;
using Wallop.Shared.Modules;
using WallopSceneEditor.Models;
using WallopSceneEditor.Models.EventData;
using WallopSceneEditor.Models.EventData.Mutator;

namespace WallopSceneEditor.Services
{



    public delegate void LayoutAdded(string layoutName);


    public interface ISceneMutator
    {
        event EventHandler<MutatorValueChangedEventArgs<object?>>? OnPropertyContextChanged;

        event EventHandler<DirectorEventArgs>? OnDirectorAdded;
        event EventHandler<ActorEventArgs>? OnActorAdded;
        event EventHandler<ActorEventArgs>? OnValidateActor;
        event EventHandler<DirectorEventArgs>? OnValidateDirector;
        event LayoutAdded? OnLayoutAdded;

        event EventHandler<MutatorValueChangedEventArgs<string>>? OnLayoutRenamed;
        event EventHandler<MutatorValueChangedEventArgs<string>>? OnDirectorRenamed;
        event EventHandler<ActorValueChangedEventArgs<string>>? OnActorRenamed;


        ISceneMutatorContext? PropertyContext { get; set; }

        void AddDirector(string modulePath, string name);
        void AddActor(string parentLayout, string modulePath, string name);

        bool RenameDirector(string directorName, string newName);
        bool RenameActor(string parentLayout, string actorName, string newName);

        StoredModule? FindActor(string parentLayout, string actorName);
        StoredLayout? FindLayout(string layoutName);
        StoredModule? FindDirector(string directorName);

        public void AddLayout(string name);
        public bool RenameLayout(string layoutName, string newName);
        void SetPropertyContextToActor(string parentLayout, string actorName);
        void SetPropertyContextToDirector(string directorName);
        void SetPropertyContextToScene(string sceneName);
        void SetPropertyContextToLayout(string layoutName);
        void ClearPropertyContext();
        void ValidatePropertyContextAsModule();
        void ValidatePropertyContextAsLayout();
    }
}
