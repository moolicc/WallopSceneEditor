using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;

namespace WallopSceneEditor.Models
{
    public class SceneContext : ISceneMutatorContext
    {
        public StoredScene Scene { get; private set; }

        public SceneContext(StoredScene scene)
        {
            Scene = scene;
        }
    }
}
