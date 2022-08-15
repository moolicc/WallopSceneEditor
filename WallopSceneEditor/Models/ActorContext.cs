using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;

namespace WallopSceneEditor.Models
{
    public class ActorContext : ISceneMutatorContext
    {
        public StoredModule RelatedModule { get; private set; }
        public string ParentLayout { get; private set; }

        public ActorContext(StoredModule module, string parentLayout)
        {
            RelatedModule = module;
            ParentLayout = parentLayout;
        }
    }
}
