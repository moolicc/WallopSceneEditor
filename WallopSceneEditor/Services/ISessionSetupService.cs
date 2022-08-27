using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    public enum SceneSources
    {
        File,
        BoundEngine,
        New,
    }

    public interface ISessionSetupService
    {
        SceneSources SceneSource { get; set; }

        int? BoundEngineProcId { get; set; }
        string? SceneFile { get; set; }


    }
}
