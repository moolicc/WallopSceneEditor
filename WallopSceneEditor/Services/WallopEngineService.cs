using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WallopSceneEditor.Models;
using System.Text.Json;
using System.IO;
using Wallop.Shared;
using Wallop.Shared.Messaging;
using Wallop.Shared.Messaging.Messages;
using Wallop.IPC;
using Wallop.Shared.Messaging.Remoting;

namespace WallopSceneEditor.Services
{
    public class WallopEngineService : IEngineService
    {
        private const string RESOURCE_MESSENGER = "msg";
        private const string RESOURCE_DELIMITER = "-";

        private const string CONF = "engine.json";

        private Process? _engineProc;
        private bool _usingExistingProcess;

        private PipeClient _msgClient;
        private RemoteMessenger _messenger;

        public void HookProcess(int pid)
        {
            _engineProc = Process.GetProcessById(pid);
            _usingExistingProcess = true;
        }

        public void StartProcess(string parentHandle, AppSettingsModel appConfig, EngineConfigModel config, DataReceivedEventHandler onOutput)
        {
            _usingExistingProcess = false;
            WriteEngineConfig(CONF, appConfig, parentHandle);

            var startInfo = new ProcessStartInfo()
            {
                FileName = appConfig.EnginePath,
                Arguments = $"[my-name:{config.InstanceName}] --configuration {CONF}",
                WorkingDirectory = Environment.CurrentDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            _engineProc = new Process();
            _engineProc.OutputDataReceived += onOutput;
            _engineProc.ErrorDataReceived += onOutput;
            _engineProc.StartInfo = startInfo;
            _engineProc.Start();
            _engineProc.BeginOutputReadLine();
        }

        public Process? GetEngineProcess()
            => _engineProc;


        public async Task<bool> ConnectAsync(string myName, string hostName, string machine = ".")
        {
            try
            {
                _msgClient = new PipeClient(myName, machine, hostName, $"{hostName}{RESOURCE_DELIMITER}{RESOURCE_MESSENGER}");
                _messenger = new RemoteMessenger(_msgClient, hostName);
                //await _msgClient.BeginAsync().ConfigureAwait(false);

                //var myId = _messenger.Put(new GraphicsMessage(100, 340));
                //var res = _messenger.TryGetReply<MessageReply>(myId, out var reply);

                //await _msgClient.EndAsync().ConfigureAwait(false);

                if(_usingExistingProcess)
                {
                    // TODO: Download the configuration from the process via sending a request.
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public void ShutdownProcess()
        {
            throw new NotImplementedException();
        }

        private void WriteEngineConfig(string filepath, AppSettingsModel appConfig, string parentHandle)
        {
            // TODO: Save configuration.
            var jsonModel = new
            {
                AppSettings = new
                {
                    InstanceName = appConfig.EngineConfig.InstanceName,
                    DependencyPaths = new[]
                    {
                        new
                        {
                            Directory = appConfig.PluginDirectory,
                            Recusive = true
                        }
                    }

                },
                SceneSettings = new
                {
                    PackageSearchDirectory = appConfig.PackageDirectory,
                },
                PluginSettings = new
                {
                },
                GraphicsSettings = new
                {
                    WindowWidth = appConfig.EngineConfig.Width,
                    ParentHandle = parentHandle,
                }
            };
            var json = JsonSerializer.Serialize(jsonModel);
            File.WriteAllText(filepath, json);
        }
    }
}
