using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;
using Wallop.Shared.Messaging.Messages;
using WallopSceneEditor.Models;

namespace WallopSceneEditor.Services
{
    public interface IEngineService
    {
        void HookProcess(int pid);
        void StartProcess(string parentHandle, AppSettingsModel appConfig, EngineConfigModel config, System.Diagnostics.DataReceivedEventHandler onOutput);
        System.Diagnostics.Process? GetEngineProcess();
        void ShutdownProcess();

        Task<bool> ConnectAsync(string myName, string hostName, string machine = ".");

        Task<bool> SendMessageAsync<T>(T message) where T : struct;
        Task<MessageReply?> SendMessageExpectReplyAsync<T>(T message)
            where T : struct;
    }
}
