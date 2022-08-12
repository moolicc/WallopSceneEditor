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
        public ObservableCollection<string> OutputList { get; set; }

        public string? AttachedProcessText
        {
            get => _attachedProcessText;
            set => this.RaiseAndSetIfChanged(ref _attachedProcessText, value);
        }

        public int SelectedOutputItem
        {
            get => _selectedOutputItem;
            set => this.RaiseAndSetIfChanged(ref _selectedOutputItem, value);
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


        public ObservableCollection<ItemViewModel> Modules { get; set; }

        private string? _attachedProcessText = "AttachedProcess:";
        private int _selectedOutputItem;

        private IDock _layout;
        private string _currentView;
        private IFactory _factory;


        private bool _loading;

        private ISettingsService _settings;
        private IEngineService _engineService;
        private IWindowService _windowService;

        public SceneEditViewModel(ISettingsService settings, IEngineService engineService, IWindowService windowService)
        {
            Modules = new System.Collections.ObjectModel.ObservableCollection<ViewModels.ItemViewModel>
            {
                new ViewModels.ItemViewModel(ViewModels.ItemTypes.Scene, "package>module", "text", "😀", "description")
            };

            _settings = settings;
            _engineService = engineService;
            _windowService = windowService;
            OutputList = new ObservableCollection<string>();
        }

        public void CreateSession(Wallop.Shared.ECS.StoredScene scene)
        {
            var appSettings = _settings.GetSettingsAsync().Result;
            SessionData = new SessionDataModel(scene, appSettings.PackageDirectory);
            SessionMutator = new SessionDataSceneMutator(SessionData);
        }

        protected override void OnActivate()
        {
            if(!_loading)
            {
                var appSettings = _settings.GetSettingsAsync().Result;
                _loading = true;
                // TODO: If we are using an existing engine instance.


                // TODO: If we're not using an existing engine instance.
                //_engineService.StartProcess(_windowService.WindowHandle.ToString(), appSettings, appSettings.EngineConfig, Proc_OutputDataReceived);
                //var proc = _engineService.GetEngineProcess()!;

                //AttachedProcessText = $"Attached: {proc.ProcessName} ({proc.Id})";

                //proc.Exited += Proc_Exited;



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
            AddToOutputList(e.Data ?? "");
        }

        private void AddToOutputList(string line)
        {
            OutputList.Add(line);
            SelectedOutputItem = OutputList.Count - 1;
        }
    }
}
