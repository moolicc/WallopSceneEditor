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
        public StoredScene LoadedScene { get; set; }

        public IEnumerable<Package> Packages { get; set; }

        public SessionDataModel(StoredScene loadedScene, string packageDir)
        {
            LoadedScene = loadedScene;
            Packages = PackageLoader.LoadPackages(packageDir);
        }
    }
}
