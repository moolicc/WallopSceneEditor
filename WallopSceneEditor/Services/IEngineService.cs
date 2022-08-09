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
        void HookProcess(int pid);
        void StartProcess(AppSettingsModel appConfig, EngineConfigModel config, System.Diagnostics.DataReceivedEventHandler onOutput);
        System.Diagnostics.Process? GetEngineProcess();
        void ShutdownProcess();

        Task<bool> ConnectAsync(string myName, string hostName, string machine = ".");
    }
}
