using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WallopSceneEditor.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using Dock.Model.Core;
using WallopSceneEditor.Models;
using Wallop.Shared.Messaging.Messages;
using Wallop.Shared.ECS;
using System.Text.Json.Nodes;
using System.Text.Json;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Controls;
using System.IO;
using Wallop.IPC;

namespace WallopSceneEditor.ViewModels
{
    public class SceneEditViewModel : ViewModelBase
    {
        public string? AttachedProcessText
        {
            get => _attachedProcessText;
            set => this.RaiseAndSetIfChanged(ref _attachedProcessText, value);
        }

        public IFactory Factory
        {
            get => _factory;
            set => this.RaiseAndSetIfChanged(ref _factory, value);
        }

        public IDock Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }

        public string CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        public bool Loaded
        {
            get => _loaded;
            set => this.RaiseAndSetIfChanged(ref _loaded, value);
        }

        public bool Loading
        {
            get => _loading;
            set => this.RaiseAndSetIfChanged(ref _loading, value);
        }

        public string LoadingText
        {
            get => _loadingText;
            set => this.RaiseAndSetIfChanged(ref _loadingText, value);
        }

        public bool LoadError
        {
            get => _loadError;
            set
            {
                this.RaiseAndSetIfChanged(ref _loadError, value);
                if(value)
                {
                    ProgressForeground = Colors.Red;
                }
            }
        }

        public Color? ProgressForeground
        {
            get => _progressForeground;
            set => this.RaiseAndSetIfChanged(ref _progressForeground, value);
        }

        public SessionDataModel SessionData { get; private set; }
        public SessionDataSceneMutator SessionMutator { get; private set; }


        private string? _attachedProcessText = "AttachedProcess:";

        private IDock _layout;
        private string _currentView;
        private IFactory _factory;


        private bool _loading;
        private bool _loaded;
        private bool _loadError;
        private string _loadingText;
        private Color? _progressForeground;


        private ISettingsService _settings;
        private IEngineService _engineService;
        private IWindowService _windowService;
        private ISessionSetupService _setup;
        private IPluginService _pluginService;

        public SceneEditViewModel(ISettingsService settings, IEngineService engineService, IWindowService windowService, ISessionSetupService setup, IPluginService pluginService)
        {
            _progressForeground = (Color?)((AvaloniaWindowService)windowService).MainWindow.FindResource("SystemAccentColor");
            _settings = settings;
            _engineService = engineService;
            _windowService = windowService;
            _setup = setup;
            _pluginService = pluginService;
        }

        protected override async Task OnActivateAsync()
        {
            if(_loading && !_loaded)
            {
                return;
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Factory.StartNew(async () =>
            {
                _loading = true;

                var stage = "";
                try
                {
                    stage = "Loading settings";
                    LoadingText = "Loading application settings...";
                    OutputHelper.Log("Loading application settings...", "", "Setup");
                    var appSettings = await _settings.GetSettingsAsync().ConfigureAwait(false);

                    if (_setup.BoundEngineProcId.HasValue)
                    {
                        stage = "Performing engine handshake";
                        if (_setup.BoundEngineProcId.Value > 0)
                        {
                            LoadingText = $"Hooking engine with PID: {_setup.BoundEngineProcId.Value}...";
                            OutputHelper.Log($"Hooking engine with PID: {_setup.BoundEngineProcId.Value}...", "", "Setup");

                            _engineService.HookProcess(_setup.BoundEngineProcId.Value);
                        }
                        else
                        {
                            LoadingText = "Launching new engine instance...";
                            OutputHelper.Log("Launching new engine instance...", "", "Setup");

                            if(!File.Exists(appSettings.EnginePath))
                            {
                                throw new FileNotFoundException("Engine executable not found.", appSettings.EnginePath);
                            }
                            _engineService.StartProcess("", appSettings, appSettings.EngineConfig, Proc_OutputDataReceived);
                        }


                        stage = "Attaching engine process";
                        var proc = _engineService.GetEngineProcess()!;
                        proc.Exited += Proc_Exited;

                        AttachedProcessText = $"Attached: {proc.ProcessName} ({proc.Id})";

                        LoadingText = "Connecting to engine's IPC endpoint...";
                        OutputHelper.Log("Connecting to engine's IPC endpoint...", "", "Setup");
                        if (!await _engineService.ConnectAsync(appSettings.ApplicationName, appSettings.EngineConfig.InstanceName).ConfigureAwait(false))
                        {
                            LoadingText = "Failed to establish IPC connection with engine!";
                            OutputHelper.Log("Failed to establish IPC connection with engine!", "", "Setup", MessageType.Error);
                            Loading = false;
                            return;
                        }
                    }


                    stage = "Loading scene";
                    LoadingText = "Setting up scene...";
                    OutputHelper.Log("Setting up scene...", "", "Setup");
                    StoredScene loadedScene;
                    if (_setup.SceneSource == SceneSources.File)
                    {
                        OutputHelper.Log("Loading scene from file...", "", "Setup");
                        loadedScene = new StoredScene() { Name = "Scene" };
                    }
                    else if (_setup.SceneSource == SceneSources.BoundEngine)
                    {
                        OutputHelper.Log("Loading scene from engine...", "", "Setup");
                        var reply = _engineService.SendMessageExpectReplyAsync(new GetSceneMessage()).Result;
                        if (reply.HasValue && reply.Value.Status == ReplyStatus.Successful && reply.Value.Content != null)
                        {
                            OutputHelper.Log("Deserializing scene...", "", "Setup");

                            var jObject = (JsonElement)reply.Value.Content;
                            loadedScene = jObject.Deserialize<StoredScene>()!;
                        }
                        else
                        {
                            LoadingText = "Failed to retrieve scene from bound engine!";
                            OutputHelper.Log("Failed to get satisfactory reply!", "", "Setup", MessageType.Error);
                            NotificationHelper.Notify(new Message("SceneEdit", "Load Error", "Failed to retrieve loaded scene from bound engine!", MessageType.Error));
                            Loading = false;
                            return;
                        }
                    }
                    else
                    {
                        OutputHelper.Log("Creating new scene...", "", "Setup");
                        loadedScene = new StoredScene() { Name = "Scene" };
                    }

                    stage = "Initiating session";
                    LoadingText = "Setting up session...";
                    OutputHelper.Log("Creating scene session...", "", "Setup");
                    SessionData = new SessionDataModel(loadedScene, appSettings.PackageDirectory);
                    SessionData.EngineProcessId = _setup.BoundEngineProcId;

                    OutputHelper.Log("Setting up scene mutator...", "", "Setup");
                    SessionMutator = new SessionDataSceneMutator(SessionData);

                    OutputHelper.Log("Hooking scene mutator...", "", "Setup");
                    NotificationHelper.HookMutator(SessionMutator);

                    if (_setup.BoundEngineProcId.HasValue)
                    {
                        OutputHelper.Log("Setting up mutation bridge...", "", "Setup");
                        SceneMutationBridge.Engine = _engineService;
                        SceneMutationBridge.Mutator = SessionMutator;
                        SceneMutationBridge.EngineTruth = SessionData.LoadedScene.Clone();
                        SceneMutationBridge.Enable();
                    }


                    stage = "Setting up environment";
                    LoadingText = "Setting up environment...";
                    OutputHelper.Log("Loading UI...", "", "Setup");
                    var factory = new MainDockFactory(SessionData, SessionMutator, _windowService, _pluginService);
                    var layout = factory.CreateLayout();
                    factory.InitLayout(layout);

                    Factory = factory;
                    Layout = layout;
                }
                catch (Exception ex)
                {
                    LoadingText = $"Failed to initialize!\nStage: {stage}\nError message: {ex.Message}";
                    OutputHelper.Log($"Failed to perform handshake with engine process! Stage: {stage}, Message: {ex.Message}", "", "Setup", MessageType.Error);
                    Loading = false;
                    LoadError = true;
                    return;
                }

                Loading = false;
                Loaded = true;
                LoadingText = "Completed!";
                OutputHelper.Log("Setup completed!", "", "Setup");
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void Proc_Exited(object? sender, EventArgs e)
        {
            AttachedProcessText = $"Process exited with code {_engineService.GetEngineProcess()!.ExitCode}.";
        }

        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.Data) || string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }
            var status = MessageType.Information;

            if(e.Data.Contains("Error"))
            {
                status = MessageType.Error;
            }
            else if(e.Data.Contains("Warn"))
            {
                status = MessageType.Warning;
            }

            OutputHelper.Log(e.Data, context: "Engine", type: status);
        }
    }
}
