using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;

namespace WallopSceneEditor.Models
{
    public class DirectorContext : ISceneMutatorContext
    {
        public StoredModule RelatedModule { get; private set; }

        public DirectorContext(StoredModule module)
        {
            RelatedModule = module;
        }
    }
}
