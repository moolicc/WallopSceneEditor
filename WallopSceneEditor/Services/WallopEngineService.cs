using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WallopSceneEditor.Models;
using System.Text.Json;
using System.IO;

namespace WallopSceneEditor.Services
{
    public class WallopEngineService : IEngineService
    {
        private const string CONF = "engine.json";

        private Process? _engineProc;

        public void StartProcess(AppSettingsModel appConfig, EngineConfigModel config, DataReceivedEventHandler onOutput)
        {
            // TODO: Save configuration.
            var jsonModel = new
            {
                AppSettings = new
                {
                    InstanceName = config.InstanceName,
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
                    WindowWidth = config.Width,
		            WindowHeight = config.Height,
                }
            };
            var json = JsonSerializer.Serialize(jsonModel);
            File.WriteAllText(CONF, json);

            var startInfo = new ProcessStartInfo()
            {
                FileName = appConfig.EnginePath,
                Arguments = $"--configuration {CONF}",
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

        public void ShutdownProcess()
        {
            throw new NotImplementedException();
        }

        public bool Connect(string instanceName)
        {
            throw new NotImplementedException();
        }
    }
}
