﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models
{
    public class AppSettingsModel
    {
        public string SceneDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scenes");
        public string PackageDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");
        public string PluginDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
    }
}
