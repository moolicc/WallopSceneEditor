using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;
using Wallop.Shared.Modules;

namespace WallopSceneEditor.Models
{
    public class SessionDataModel
    {
        public int? EngineProcessId { get; set; }

        public StoredScene LoadedScene { get; set; }
        public IEnumerable<Package> Packages { get; set; }

        public SessionDataModel(StoredScene loadedScene, string packageDir)
        {
            LoadedScene = loadedScene;
            Packages = PackageLoader.LoadPackages(packageDir);
        }

        public Module? FindModule(StoredModule stored)
        {
            foreach (var pkg in Packages)
            {
                foreach (var mod in pkg.DeclaredModules)
                {
                    if(mod.ModuleInfo.Id == stored.ModuleId)
                    {
                        return mod;
                    }
                }
            }
            return null;
        }
    }
}
