using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models
{
    class AppSettingsModel
    {
        public string SceneDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
    }
}
