using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.Services
{
    interface ISettingsService
    {
        Settings.ApplicationSettings AppSettings { get; }
        Settings.EngineSettings EngineSettings { get; }
        Settings.SharedSettings SharedSettings { get; }

        void Load();
        void Save();
    }
}
