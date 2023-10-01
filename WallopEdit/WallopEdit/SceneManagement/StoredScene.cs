using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.SceneManagement
{
    public class StoredScene
    {
        public Version EditorVersion { get; set; }
        public Version EngineVersion { get; set; }
        public string SceneName { get; set; }
        public string? ScreenShotFile { get; set; }

        public StoredScene()
        {
            EditorVersion = new Version();
            EngineVersion = new Version();
            SceneName = "";
            ScreenShotFile = null;
        }
    }
}
