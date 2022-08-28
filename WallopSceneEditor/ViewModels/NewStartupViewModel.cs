using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Services;
using WallopSceneEditor.Views;

namespace WallopSceneEditor.ViewModels
{
    public class NewStartupViewModel : ViewModelBase
    {
        private const int PROC_INTERVAL = 10;
        private const int PROC_NONE = -1;
        private const int PROC_NEW = -2;

        public ObservableCollection<SourceItemViewModel> Sources { get; set; }
        public ObservableCollection<EngineItemViewModel> Engines { get; set; }

        public EngineItemViewModel? SelectedEngine
        {
            get => _selectedEngine;
            set => this.RaiseAndSetIfChanged(ref _selectedEngine, value);
        }


        private DispatcherTimer _timer;
        private EngineItemViewModel? _selectedEngine;

        private bool _enginesLock;

        private readonly ISettingsService _settingsService;
        private readonly IWindowService _windowService;
        private readonly ISceneService _sceneService;
        private readonly ISessionSetupService _setup;


        public NewStartupViewModel(ISettingsService settingsService, IWindowService windowService, ISceneService sceneService, ISessionSetupService setup)
        {
            _settingsService = settingsService;
            _windowService = windowService;
            _sceneService = sceneService;
            _setup = setup;

            Sources = new ObservableCollection<SourceItemViewModel>();
            Engines = new ObservableCollection<EngineItemViewModel>();

            _timer = new DispatcherTimer(TimeSpan.FromSeconds(PROC_INTERVAL), DispatcherPriority.Normal, UpdateEngineTimer_Ticked);
        }

        protected override void OnActivate()
        {
            // TODO: Load these from plugin.
            var fileSource = new SourceItemViewModel();
            fileSource.Header = "File";
            fileSource.Description = "Opens a scene from a saved scene file.";
            fileSource.IconSymbol = FluentAvalonia.UI.Controls.Symbol.OpenFile;
            fileSource.ClickCommand = ReactiveCommand.Create(SetupEdit_File);

            var boundEngineSource = new SourceItemViewModel();
            boundEngineSource.Header = "Engine";
            boundEngineSource.Description = "Opens a scene from the selected engine.";
            boundEngineSource.IconSymbol = FluentAvalonia.UI.Controls.Symbol.Scan;
            boundEngineSource.ClickCommand = ReactiveCommand.Create(
                SetupEdit_Engine,
                this.WhenAnyValue(
                    x => x.SelectedEngine,
                    engine => engine != null &&
                    engine.ProcessId.HasValue &&
                    engine.ProcessId.Value != PROC_NONE));

            Sources.Add(fileSource);
            Sources.Add(boundEngineSource);


            Engines.Add(new EngineItemViewModel("None", PROC_NONE));
            Engines.Add(new EngineItemViewModel("New", PROC_NEW));
            SelectedEngine = Engines[0];

            LoadEngineProcs();
            _timer.Start();

            base.OnActivate();
        }

        private void LoadEngineProcs()
        {
            if(_enginesLock)
            {
                return;
            }
            _enginesLock = true;
            var processes = Process.GetProcessesByName("wallop");
            // TODO: Get active scene name of process.

            foreach (var proc in processes)
            {
                if (!Engines.Any(p => p.ProcessId == proc.Id))
                {
                    Engines.Add(new EngineItemViewModel(proc.ProcessName, proc.Id));
                }
            }

            for (int i = 0; i < Engines.Count; i++)
            {
                EngineItemViewModel? listedProc = Engines[i];
                if (listedProc.ProcessId.HasValue && listedProc.ProcessId > 0 && !processes.Any(p => p.Id == listedProc.ProcessId))
                {
                    Engines.RemoveAt(i);
                    i--;
                }
            }
            _enginesLock = false;
        }

        private void UpdateEngineTimer_Ticked(object? sender, EventArgs e)
        {
            LoadEngineProcs();
        }

        private void SetupEdit_File()
        {
            if (_selectedEngine == null || _selectedEngine.ProcessId <= 0)
            {
                return;
            }

            _setup.BoundEngineProcId = null;
            _setup.SceneFile = null;
            _setup.SceneSource = SceneSources.File;

            if (_selectedEngine.ProcessId == PROC_NEW)
            {
                _setup.BoundEngineProcId = 0;
            }
            else if(_selectedEngine.ProcessId != PROC_NONE)
            {
                _setup.BoundEngineProcId = _selectedEngine.ProcessId;
            }

            _windowService.SwitchView<SceneEditViewModel>("main");
        }

        private void SetupEdit_Engine()
        {
            if(_selectedEngine == null)
            {
                return;
            }

            _setup.BoundEngineProcId = null;
            _setup.SceneFile = null;
            _setup.SceneSource = SceneSources.BoundEngine;

            if(_selectedEngine.ProcessId == PROC_NEW)
            {
                _setup.BoundEngineProcId = 0;
            }
            else
            {
                _setup.BoundEngineProcId = _selectedEngine.ProcessId;
            }

            _windowService.SwitchView<SceneEditViewModel>("main");
        }

        public async Task ShowSettings()
        {
            var vm = _windowService.ResolveView_Inject<SettingsViewModel>();
            var content = new SettingsView();
            content.ViewModel = vm;

            var dialog = new FluentAvalonia.UI.Controls.ContentDialog();
            dialog.Content = content;
            dialog.PrimaryButtonText = "Ok";
            dialog.CloseButtonText = "Cancel";

            var result = await dialog.ShowAsync().ConfigureAwait(false);
            if(result == FluentAvalonia.UI.Controls.ContentDialogResult.Primary)
            {
                vm.OkCommand.Execute(null);
            }
        }
    }
}
