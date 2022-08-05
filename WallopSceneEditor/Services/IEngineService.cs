using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Models;

namespace WallopSceneEditor.Services
{
    public interface IEngineService
    {

        void StartProcess(AppSettingsModel appConfig, EngineConfigModel config, System.Diagnostics.DataReceivedEventHandler onOutput);
        System.Diagnostics.Process? GetEngineProcess();
        void ShutdownProcess();

        bool Connect(string instanceName);


    }
}
