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

        public SessionDataModel SessionData { get; private set; }
        public SessionDataSceneMutator SessionMutator { get; private set; }


        private string? _attachedProcessText = "AttachedProcess:";

        private IDock _layout;
        private string _currentView;
        private IFactory _factory;


        private bool _loading;

        private ISettingsService _settings;
        private IEngineService _engineService;
        private IWindowService _windowService;

        public SceneEditViewModel(ISettingsService settings, IEngineService engineService, IWindowService windowService)
        {
            _settings = settings;
            _engineService = engineService;
            _windowService = windowService;
        }

        public void CreateSession(Wallop.Shared.ECS.StoredScene scene, int? engineProcId)
        {
            var appSettings = _settings.GetSettingsAsync().Result;
            SessionData = new SessionDataModel(scene, appSettings.PackageDirectory);
            SessionData.EngineProcessId = engineProcId;
            SessionMutator = new SessionDataSceneMutator(SessionData);
            NotificationHelper.HookMutator(SessionMutator);
        }

        protected override void OnActivate()
        {
            if(!_loading)
            {
                OutputHelper.Log<SceneEditViewModel>("Beginning setup");
                var appSettings = _settings.GetSettingsAsync().Result;

                _loading = true;
                // TODO: If we are using an existing engine instance.
                
                // TODO: Allow layout activation/deactivation key/value property setting.

                // TODO: Add module dependency verification for incoming StoredScene as well as list of packages.

                // TODO: Allow editing without a backing engine instance.


                // TODO: If we're not using an existing engine instance.
                //_engineService.StartProcess(_windowService.WindowHandle.ToString(), appSettings, appSettings.EngineConfig, Proc_OutputDataReceived);


                // Setup communication with the Engine.
                if(SessionData.EngineProcessId != null)
                {
                    _engineService.HookProcess(SessionData.EngineProcessId.Value);
                }
                else
                {
                    _engineService.StartProcess("", appSettings, appSettings.EngineConfig, Proc_OutputDataReceived);
                }
                var proc = _engineService.GetEngineProcess()!;
                proc.Exited += Proc_Exited;

                _engineService.ConnectAsync(appSettings.ApplicationName, appSettings.EngineConfig.InstanceName);

                AttachedProcessText = $"Attached: {proc.ProcessName} ({proc.Id})";

                SceneMutationBridge.Engine = _engineService;
                SceneMutationBridge.Mutator = SessionMutator;
                SceneMutationBridge.EngineTruth = new Wallop.Shared.ECS.StoredScene();
                SceneMutationBridge.Enable();

                //_engineService.ConnectAsync(appSettings.ApplicationName, appSettings.EngineConfig.InstanceName);
            }
            base.OnActivate();
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
            OutputHelper.Log(e.Data, context: "Engine");
        }
    }
}
