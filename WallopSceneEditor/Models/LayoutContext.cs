using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;

namespace WallopSceneEditor.Models
{
    public class LayoutContext : ISceneMutatorContext
    {
        public StoredLayout Layout { get; private set; }

        public LayoutContext(StoredLayout layout)
        {
            Layout = layout;
        }
    }
}
