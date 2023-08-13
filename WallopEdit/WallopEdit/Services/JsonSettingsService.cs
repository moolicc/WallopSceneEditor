using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopEdit.Settings;

namespace WallopEdit.Services
{
    internal class JsonSettingsService : ISettingsService
    {
        const string SETTINGS_FILE = "wedit.json";

        [NotNull]
        public ApplicationSettings AppSettings { get; private set; }

        [NotNull]
        public EngineSettings EngineSettings { get; private set; }

        [NotNull]
        public SharedSettings SharedSettings { get; private set; }

        public JsonSettingsService()
        {
            AppSettings = new ApplicationSettings();
            EngineSettings = new EngineSettings();
            SharedSettings = new SharedSettings();
        }

        public void Load()
        {
        }

        public void Save()
        {
        }
    }
}
