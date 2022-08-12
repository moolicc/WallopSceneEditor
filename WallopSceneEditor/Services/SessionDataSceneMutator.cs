using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;
using Wallop.Shared.Modules;
using WallopSceneEditor.Models;
namespace WallopSceneEditor.Services
{
    public class SessionDataSceneMutator : ISceneMutator
    {
        public event EventHandler<DirectorEventArgs>? OnDirectorAdded;
        public event EventHandler<ActorEventArgs>? OnActorAdded;
        public event LayoutAdded? OnLayoutAdded;

        private SessionDataModel _sessionData;

        public SessionDataSceneMutator(SessionDataModel sessionData)
        {
            _sessionData = sessionData;
        }

        public void AddActor(string parentLayout, string modulePath, string name)
        {
            var moduleInfo = BreakdownModulePath(modulePath);

            var failed = true;
            var messages = new List<string>();

            if (moduleInfo.HasValue)
            {
                var package = _sessionData.Packages.FirstOrDefault(p => p.Info.ManifestPath == moduleInfo.Value.Package);

                if (package != null)
                {
                    var module = package.DeclaredModules.FirstOrDefault(m => m.ModuleInfo.ScriptName == moduleInfo.Value.Module);

                    if (module != null)
                    {
                        var newActor = new StoredModule()
                        {
                            InstanceName = name,
                            ModuleId = module.ModuleInfo.Id
                        };
                        
                        if(ValidateSettings(newActor.Settings, module.ModuleSettings))
                        {
                            FindLayout(parentLayout, out _)?.ActorModules.Add(newActor);
                            failed = false;
                        }
                        else
                        {
                            messages.Add("Missing one or more required settings.");
                        }

                    }
                    else
                    {
                        messages.Add("Module not found.");
                    }
                }
                else
                {
                    messages.Add("Package not found.");
                }
            }
            else
            {
                messages.Add("Module not specified.");
                moduleInfo = ("", "");
            }

            OnActorAdded?.Invoke(this, new ActorEventArgs(moduleInfo.Value.Package, moduleInfo.Value.Module, parentLayout, name, failed, messages.ToArray()));
        }

        public void AddDirector(string modulePath, string name)
        {
            var moduleInfo = BreakdownModulePath(modulePath);

            var failed = true;
            var messages = new List<string>();

            if (moduleInfo.HasValue)
            {
                var package = _sessionData.Packages.FirstOrDefault(p => p.Info.PackageName == moduleInfo.Value.Package);

                if (package != null)
                {
                    var module = package.DeclaredModules.FirstOrDefault(m => m.ModuleInfo.ScriptName == moduleInfo.Value.Module);

                    if (module != null)
                    {
                        var newDirector = new StoredModule()
                        {
                            InstanceName = name,
                            ModuleId = module.ModuleInfo.Id
                        };

                        if (ValidateSettings(newDirector.Settings, module.ModuleSettings))
                        {
                            _sessionData.LoadedScene.DirectorModules.Add(newDirector);
                            failed = false;
                        }
                        else
                        {
                            messages.Add("Missing one or more required settings.");
                        }
                    }
                    else
                    {
                        messages.Add("Module not found.");
                    }
                }
                else
                {
                    messages.Add("Package not found.");
                }
            }
            else
            {
                messages.Add("Module not specified.");
                moduleInfo = ("", "");
            }

            OnDirectorAdded?.Invoke(this, new DirectorEventArgs(moduleInfo.Value.Package, moduleInfo.Value.Module, name, failed, messages.ToArray()));
        }

        public void AddLayout(string name)
        {
            _sessionData.LoadedScene.Layouts.Add(new StoredLayout() { Name = name });
            OnLayoutAdded?.Invoke(name);
        }

        public void RenameDirector(string directorName, string newName)
        {
            var currentDirector = FindDirector(directorName, out _);
            if (currentDirector != null)
            {
                currentDirector.InstanceName = newName;
            }
        }

        public void RenameActor(string parentLayout, string actorName, string newName)
        {
            var currentActor = FindActor(parentLayout, actorName, out _, out _);
            if(currentActor != null)
            {
                currentActor.InstanceName = newName;
            }
        }

        public void RenameLayout(string layoutName, string newName)
        {
            var currentLayout = FindLayout(layoutName, out _);
            if (currentLayout != null)
            {
                currentLayout.Name = newName;
            }
        }


        private bool ValidateSettings(List<StoredSetting> activeSettings, IEnumerable<ModuleSetting> moduleSettings)
        {
            foreach (var item in moduleSettings)
            {
                if(item.Required)
                {
                    if(!activeSettings.Any(a => a.Name == item.SettingName))
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
    }
}
