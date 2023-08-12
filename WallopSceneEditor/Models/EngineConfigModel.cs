using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models
{
    public class EngineConfigModel
    {
        public string InstanceName { get; set; } = "editorhost";
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;

        public string LogFile { get; set; } = "editor_engine.log";

    }
}
