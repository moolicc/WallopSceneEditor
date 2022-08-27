using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    internal class SessionSetupService : ISessionSetupService
    {
        public SceneSources SceneSource { get; set; }
        public int? BoundEngineProcId { get; set; }
        public string? SceneFile { get; set; }
    }
}
