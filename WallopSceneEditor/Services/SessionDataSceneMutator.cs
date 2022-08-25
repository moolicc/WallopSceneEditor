using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wallop.Shared.ECS;
using Wallop.Shared.Modules;
using WallopSceneEditor.Models;
using WallopSceneEditor.Models.EventData;
using WallopSceneEditor.Models.EventData.Mutator;
using static Avalonia.Media.Transformation.TransformOperation.DataLayout;

namespace WallopSceneEditor.Services
{
    public class SessionDataSceneMutator : ISceneMutator
    {
        public event EventHandler<MutatorValueChangedEventArgs<object?>>? OnPropertyContextChanged;

        public event EventHandler<LayoutEventArgs>? OnLayoutAdded;
        public event EventHandler<DirectorEventArgs>? OnDirectorAdded;
        public event EventHandler<ActorEventArgs>? OnActorAdded;
        public event EventHandler<LayoutEventArgs>? OnValidatedLayout;
        public event EventHandler<ActorEventArgs>? OnValidateActor;
        public event EventHandler<DirectorEventArgs>? OnValidateDirector;

        public event EventHandler<MutatorValueChangedEventArgs<string>>? OnLayoutRenamed;
        public event EventHandler<MutatorValueChangedEventArgs<string>>? OnDirectorRenamed;
        public event EventHandler<ActorValueChangedEventArgs<string>>? OnActorRenamed;

        public ISceneMutatorContext? PropertyContext
        {
            get => _propertyContext;
            set
            {
                if(_propertyContext != value)
                {
                    var oldValue = _propertyContext;
                    _propertyContext = value;
                    OnPropertyContextChanged?.Invoke(this, new MutatorValueChangedEventArgs<object?>(oldValue, value));
                }
            }
        }


        private SessionDataModel _sessionData;
        private ISceneMutatorContext? _propertyContext;


        public SessionDataSceneMutator(SessionDataModel sessionData)
        {
            _sessionData = sessionData;
        }

        public void AddActor(string parentLayout, string modulePath, string name)
        {
            var moduleInfo = BreakdownModulePath(modulePath);

            var failed = true;
            var messages = new List<Message>();
            var newActor = new StoredModule()
            {
                InstanceName = name
            };

            if (moduleInfo.HasValue)
            {
                var package = _sessionData.Packages.FirstOrDefault(p => p.Info.ManifestPath == moduleInfo.Value.Package);

                if (package != null)
                {
                    var module = package.DeclaredModules.FirstOrDefault(m => m.ModuleInfo.ScriptName == moduleInfo.Value.Module);

                    if (module != null)
                    {
                        newActor.ModuleId = module.ModuleInfo.Id;
                        if (ValidateSettings(newActor.Settings, module.ModuleSettings))
                        {
                            failed = false;
                        }
                        else
                        {
                            messages.Add(new Message("Actor", name, "Missing one or more required settings.", MessageType.Warning));
                        }

                    }
                    else
                    {
                        messages.Add(new Message("Actor", name, "Module not found.", MessageType.Warning));
                    }
                }
                else
                {
                    messages.Add(new Message("Actor", name, "Package not found.", MessageType.Warning));
                }
            }
            else
            {
                messages.Add(new Message("Actor", name, "Module not specified.", MessageType.Warning));
                moduleInfo = ("", "");
            }

            FindLayout(parentLayout, out _)?.ActorModules.Add(newActor);
            OnActorAdded?.Invoke(this, new ActorEventArgs(moduleInfo.Value.Package, moduleInfo.Value.Module, parentLayout, name, failed, messages.ToArray()));
        }

        public void AddDirector(string modulePath, string name)
        {
            var moduleInfo = BreakdownModulePath(modulePath);

            var failed = true;
            var messages = new List<Message>();

            var newDirector = new StoredModule()
            {
                InstanceName = name,
            };

            if (moduleInfo.HasValue)
            {
                var package = _sessionData.Packages.FirstOrDefault(p => p.Info.PackageName == moduleInfo.Value.Package);

                if (package != null)
                {
                    var module = package.DeclaredModules.FirstOrDefault(m => m.ModuleInfo.ScriptName == moduleInfo.Value.Module);

                    if (module != null)
                    {
                        newDirector.ModuleId = module.ModuleInfo.Id;

                        if (ValidateSettings(newDirector.Settings, module.ModuleSettings))
                        {
                            failed = false;
                        }
                        else
                        {
                            messages.Add(new Message("Director", name, "Missing one or more required settings.", MessageType.Warning));
                        }
                    }
                    else
                    {
                        messages.Add(new Message("Director", name, "Module not found.", MessageType.Warning));
                    }
                }
                else
                {
                    messages.Add(new Message("Director", name, "Package not found.", MessageType.Warning));
                }
            }
            else
            {
                messages.Add(new Message("Director", name, "Module not specified.", MessageType.Warning));
                moduleInfo = ("", "");
            }

            _sessionData.LoadedScene.DirectorModules.Add(newDirector);
            OnDirectorAdded?.Invoke(this, new DirectorEventArgs(moduleInfo.Value.Package, moduleInfo.Value.Module, name, failed, messages.ToArray()));
        }

        public void AddLayout(string name)
        {
            var newLayout = new StoredLayout();
            newLayout.Name = name;
            newLayout.RenderWidth = 0;
            newLayout.RenderHeight = 0;
            newLayout.ScreenIndex = 0;

            var screens = AvaloniaWindowService.Instance.MainWindow.Screens;

            var messages = new List<Message>();
            var failed = true;

            if (newLayout.ScreenIndex > screens.ScreenCount + 1)
            {
                messages.Add(new Message("Layout", newLayout.Name, "Invalid screen specified.", MessageType.Warning));
            }
            else
            {
                Avalonia.PixelRect bounds;
                if (newLayout.ScreenIndex == 0)
                {
                    // TODO: build-up bounds.
                    bounds = new Avalonia.PixelRect();
                }
                else
                {
                    bounds = screens.All[newLayout.ScreenIndex - 1].Bounds;
                }

                if (newLayout.RenderWidth > bounds.Width
                    || newLayout.RenderHeight > bounds.Height
                    || newLayout.RenderWidth <= 0
                    || newLayout.RenderHeight <= 0)
                {
                    messages.Add(new Message("Layout", newLayout.Name, "Invalid render space specified.", MessageType.Warning));
                }
                else
                {
                    failed = false;
                }
            }

            _sessionData.LoadedScene.Layouts.Add(new StoredLayout() { Name = name });
            OnLayoutAdded?.Invoke(this, new LayoutEventArgs(name) {  HasError = failed, Messages = messages });
        }

        public bool RenameDirector(string directorName, string newName)
        {
            var currentDirector = FindDirector(directorName, out _);
            var newDirector = FindDirector(directorName, out _);

            var result = false;
            var messages = new List<Message>();

            if (currentDirector != null && newDirector == null)
            {
                currentDirector.InstanceName = newName;
                result = true;
            }
            else if (currentDirector == null)
            {
                messages.Add(new Message("Director", directorName, "Director not found.", MessageType.Warning));
            }
            else if (newDirector != null)
            {
                messages.Add(new Message("Director", directorName, "A director with that name already exists.", MessageType.Warning));
            }

            OnDirectorRenamed?.Invoke(this, new MutatorValueChangedEventArgs<string>(directorName, newName, !result) { Messages = messages });

            return result;
        }

        public bool RenameActor(string parentLayout, string actorName, string newName)
        {
            var currentActor = FindActor(parentLayout, actorName, out _, out _);
            var newActor = FindActor(parentLayout, newName, out _, out _);

            var result = false;
            var messages = new List<Message>();

            if (currentActor != null && newActor == null)
            {
                currentActor.InstanceName = newName;
                result = true;
            }
            else if (currentActor == null)
            {
                messages.Add(new Message("Actor", actorName, "Actor not found.", MessageType.Warning));
            }
            else if (newActor != null)
            {
                messages.Add(new Message("Actor", actorName, "An actor with that name already exists.", MessageType.Warning));
            }

            OnActorRenamed?.Invoke(this, new ActorValueChangedEventArgs<string>(actorName, newName, !result, parentLayout) { Messages = messages });

            return result;
        }

        public bool RenameLayout(string layoutName, string newName)
        {
            var currentLayout = FindLayout(layoutName, out _);
            var newLayout = FindLayout(newName, out _);

            var result = false;
            var messages = new List<Message>();

            if (currentLayout != null && newLayout == null)
            {
                currentLayout.Name = newName;
                result = true;
            }
            else if(currentLayout == null)
            {
                messages.Add(new Message("Layout", layoutName, "Layout not found.", MessageType.Warning));
            }
            else if (newLayout != null)
            {
                messages.Add(new Message("Layout", layoutName, "A layout with that name already exists.", MessageType.Warning));
            }

            OnLayoutRenamed?.Invoke(this, new MutatorValueChangedEventArgs<string>(layoutName, newName, !result) { Messages = messages } );
            return result;
        }
        public StoredModule? FindActor(string parentLayout, string actorName)
            => FindActor(parentLayout, actorName, out _, out _);

        public StoredLayout? FindLayout(string layoutName)
            => FindLayout(layoutName, out _);

        public StoredModule? FindDirector(string directorName)
            => FindDirector(directorName, out _);

        private bool ValidateSettings(List<StoredSetting> activeSettings, IEnumerable<ModuleSetting> moduleSettings)
        {
            // TODO: If a setting is set to null, check the upcoming module.Nullable.
            foreach (var item in moduleSettings)
            {
                if(item.Required)
                {
                    if(!activeSettings.Any(a => a.Name == item.SettingName && a.Value != null))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private (string Package, string Module)? BreakdownModulePath(string path)
        {
            if (!path.Contains('>'))
            {
                return null;
            }

            var split = path.Split(new[] { '>' }, 2);
            return (split[0], split[1]);
        }

        private StoredLayout? FindLayout(string name, out int index)
        {
            for (int i = 0; i < _sessionData.LoadedScene.Layouts.Count; i++)
            {
                if (_sessionData.LoadedScene.Layouts[i].Name == name)
                {
                    index = i;
                    return _sessionData.LoadedScene.Layouts[i];
                }
            }

            index = -1;
            return null;
        }

        private StoredModule? FindDirector(string name, out int index)
        {
            for (int i = 0; i < _sessionData.LoadedScene.DirectorModules.Count; i++)
            {
                if(_sessionData.LoadedScene.DirectorModules[i].InstanceName == name)
                {
                    index = i;
                    return _sessionData.LoadedScene.DirectorModules[i];
                }
            }

            index = -1;
            return null;
        }

        private StoredModule? FindActor(string owningLayout, string name, out int layoutIndex, out int index)
        {
            var layout = FindLayout(owningLayout, out layoutIndex);
            if(layout != null)
            {
                for (int i = 0; i < layout.ActorModules.Count; i++)
                {
                    if (layout.ActorModules[i].InstanceName == name)
                    {
                        index = i;
                        return layout.ActorModules[i];
                    }
                }
            }

            index = -1;
            return null;
        }


        public void SetPropertyContextToActor(string parentLayout, string actorName)
        {
            PropertyContext = new ActorContext(FindActor(parentLayout, actorName, out _, out _), parentLayout);
        }

        public void SetPropertyContextToDirector(string directorName)
        {
            PropertyContext = new DirectorContext(FindDirector(directorName, out _));
        }

        public void SetPropertyContextToScene(string sceneName)
        {
            PropertyContext = new SceneContext(_sessionData.LoadedScene);
        }

        public void SetPropertyContextToLayout(string layoutName)
        {
            PropertyContext = new LayoutContext(FindLayout(layoutName, out _));
        }

        public void ValidatePropertyContextAsModule()
        {
            if(PropertyContext is ActorContext actor)
            {
                ValidatePropertyContextAsActor(actor);
            }
            else if(PropertyContext is DirectorContext director)
            {
                ValidatePropertyContextAsDirector(director);
            }
        }

        public void ValidatePropertyContextAsLayout()
        {
            // TODO: Validate screen, render size, and presentation bounds.
            if(AvaloniaWindowService.Instance == null)
            {
                return;
            }

            var failed = true;
            var messages = new List<Message>();
            if (PropertyContext is LayoutContext layout)
            {
                var screens = AvaloniaWindowService.Instance.MainWindow.Screens;

                if (layout.Layout.ScreenIndex > screens.ScreenCount + 1)
                {
                    messages.Add(new Message("Layout", layout.Layout.Name, "Invalid screen specified.", MessageType.Warning));
                }
                else
                {
                    Avalonia.PixelRect bounds;
                    if (layout.Layout.ScreenIndex == 0)
                    {
                        // TODO: build-up bounds.
                        bounds = new Avalonia.PixelRect();
                    }
                    else
                    {
                        bounds = screens.All[layout.Layout.ScreenIndex - 1].Bounds;
                    }

                    if (layout.Layout.RenderWidth > bounds.Width
                        || layout.Layout.RenderHeight > bounds.Height
                        || layout.Layout.RenderWidth <= 0
                        || layout.Layout.RenderHeight <= 0)
                    {
                        messages.Add(new Message("Layout", layout.Layout.Name, "Invalid render space specified.", MessageType.Warning));
                    }
                    else
                    {
                        failed = false;
                    }
                }

                OnValidatedLayout?.Invoke(this, new LayoutEventArgs(layout.Layout.Name) { HasError = failed, Messages = messages });
            }

        }

        public void ClearPropertyContext()
        {
            PropertyContext = null;
        }

        private void ValidatePropertyContextAsActor(ActorContext actor)
        {
            var moduleInfo = FindModuleInfo(actor.RelatedModule.ModuleId);

            var failed = true;
            var messages = new List<Message>();

            if (moduleInfo.HasValue)
            {
                if (ValidateSettings(actor.RelatedModule.Settings, moduleInfo.Value.Module.ModuleSettings))
                {
                    failed = false;
                }
                else
                {
                    messages.Add(new Message("Actor", actor.RelatedModule.InstanceName, "Missing one or more required settings.", MessageType.Warning));
                }
            }
            else
            {
                messages.Add(new Message("Actor", actor.RelatedModule.InstanceName, "Module or package not found.", MessageType.Warning));
            }

            OnValidateActor?.Invoke(this, new ActorEventArgs(moduleInfo?.Package.Info.ManifestPath ?? "", actor.RelatedModule.ModuleId, actor.ParentLayout, actor.RelatedModule.InstanceName, failed, messages.ToArray()));
        }

        private void ValidatePropertyContextAsDirector(DirectorContext director)
        {
            var moduleInfo = FindModuleInfo(director.RelatedModule.ModuleId);

            var failed = true;
            var messages = new List<Message>();

            if (moduleInfo.HasValue)
            {
                if (ValidateSettings(director.RelatedModule.Settings, moduleInfo.Value.Module.ModuleSettings))
                {
                    failed = false;
                }
                else
                {
                    messages.Add(new Message("Director", director.RelatedModule.InstanceName, "Missing one or more required settings.", MessageType.Warning));
                }
            }
            else
            {
                messages.Add(new Message("Director", director.RelatedModule.InstanceName, "Module or package not found.", MessageType.Warning));
            }

            OnValidateDirector?.Invoke(this, new DirectorEventArgs(moduleInfo?.Package.Info.ManifestPath ?? "", director.RelatedModule.ModuleId, director.RelatedModule.InstanceName, failed, messages.ToArray()));
        }

        private (Package Package, Module Module)? FindModuleInfo(string module)
        {
            foreach (var pkg in _sessionData.Packages)
            {
                foreach (var mod in pkg.DeclaredModules)
                {
                    if (mod.ModuleInfo.Id == module)
                    {
                        return (pkg, mod);
                    }
                }
            }
            return null;
        }

    }
}
