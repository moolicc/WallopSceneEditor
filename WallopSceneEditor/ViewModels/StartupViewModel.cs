using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WallopSceneEditor.Models;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.ViewModels
{
    public class RecentFileViewModel
    {
        public static readonly RecentFileViewModel Empty = new RecentFileViewModel("Loading ...", "");

        public string FilePath { get; set; }
        public string SceneName { get; set; }
        public string Message { get; set; }
        public bool HasErrors { get; set; }


        public RecentFileViewModel(string filePath, string sceneName)
        {
            FilePath = filePath;
            SceneName = sceneName;
            Message = sceneName;
            HasErrors = false;
        }

        public RecentFileViewModel(string filePath, string errorMessage, bool hasErrors)
        {
            FilePath = filePath;
            SceneName = "";
            Message = errorMessage;
            HasErrors = hasErrors;
        }
    }


    public class StartupViewModel : ViewModelBase
    {
        public string? SceneName
        {
            get => _sceneName;
            set => this.RaiseAndSetIfChanged(ref _sceneName, value);
        }
        private string? _sceneName = "New scene";


        public string? SceneFileName
        {
            get => _sceneFileName;
            set
            {
                this.RaiseAndSetIfChanged(ref _sceneFileName, value);

                if (_updateSelectedByFileName)
                {
                    _updateSelectedByFileName = false;
                    SetSelectedFileByName();
                }
                _updateSelectedByFileName = true;
            }
        }
        private string? _sceneFileName = "";


        public string? SelectedFile
        {
            get => _selectedFile;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFile, value);
                FileSelected(null);
            }
        }
        private string? _selectedFile;


        public string? ButtonText
        {
            get => _buttonText;
            set => this.RaiseAndSetIfChanged(ref _buttonText, value);
        }
        private string? _buttonText = "Load scene";

        public bool? ButtonEnabled
        {
            get => _buttonEnabled;
            set => this.RaiseAndSetIfChanged(ref _buttonEnabled, value);
        }
        private bool? _buttonEnabled = false;

        public RecentFileViewModel[] RecentItems
        {
            get => _recentFiles;
            set => this.RaiseAndSetIfChanged(ref _recentFiles, value);
        }
        private RecentFileViewModel[] _recentFiles = new[] { RecentFileViewModel.Empty };

        public int SelectedTab
        {
            get => _selectedTab;
            set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }
        private int _selectedTab = 0;



        public ObservableCollection<ProcessItemViewModel> OtherProcesses { get; set; }

        public string OtherProcSceneName
        {
            get => _otherProcSceneName;
            set => this.RaiseAndSetIfChanged(ref _otherProcSceneName, value);
        }
        private string _otherProcSceneName = "";


        public ICommand BeginEditCommand { get; }
        public ICommand ShowSettingsCommand { get; }


        private bool _updateSelectedByFileName = true;

        private ISettingsService _settingsService;
        private IWindowService _windowService;
        private ISceneService _sceneService;
        private bool _loadingRecents;
        private bool _deactivating;

        internal StartupViewModel(ISettingsService settingsService, IWindowService windowService, ISceneService sceneService)
        {
            SelectedFile = settingsService.GetSettingsAsync().Result.SceneDirectory;
            _settingsService = settingsService;
            _windowService = windowService;
            _sceneService = sceneService;
            OtherProcesses = new ObservableCollection<ProcessItemViewModel>();

            _deactivating = false;

            BeginEditCommand = ReactiveCommand.Create(() =>
            {
                if(SceneName == null)
                {
                    // TODO: Show error
                    // Failed.
                    return;
                }
                var vm = _windowService.ResolveView_Inject<SceneEditViewModel>();
                vm.Scene = _sceneService.CreateScene(SceneName, SelectedFile);

                _windowService.SwitchView("main", vm);
            });

            ShowSettingsCommand = ReactiveCommand.Create(async () =>
            {
                var vm = _windowService.ResolveView_Inject<SettingsViewModel>();
                var dialog = new Views.SettingsView();
                dialog.ViewModel = vm;

                var result = await dialog.ShowDialog<bool?>((_windowService as AvaloniaWindowService)!.MainWindow);
            });

        }

        public void FileSelected(Views.FilePicker? picker)
        {
            if(!File.Exists(SelectedFile))
            {
                ButtonText = "Create scene";
            }
            else
            {
                ButtonText = "Load scene";
                SceneFileName = Path.GetFileName(SelectedFile);
            }

            if (Directory.Exists(SelectedFile))
            {
                ButtonEnabled = false;
            }
            else
            {
                ButtonEnabled = true;
            }
        }

        public void OnListSelectedItem(RecentFileViewModel? item)
        {
            if(item != null && item != RecentFileViewModel.Empty && !item.HasErrors)
            {
                SceneName = item.SceneName;
                SelectedFile = item.FilePath;
            }

            SelectedTab = 0;
        }

        public void OnOtherProcessSelectedItem(ProcessItemViewModel? item)
        {
            if (item != null)
            {
                // TODO: Resolve scene name.

                OtherProcSceneName = "Loading ...";
            }
            else
            {
                OtherProcSceneName = "";
            }
        }

        protected override void OnActivate()
        {
            LoadRecentFiles();
            SetProcessList();
        }

        private void LoadRecentFiles()
        {
            if (!_loadingRecents)
            {
                _loadingRecents = true;
                _windowService.ScheduleOnUIThread(async () =>
                {
                    var recent = (await _settingsService.GetRecentFilesAsync()).RecentFiles.ToArray();
                    var results = new RecentFileViewModel[recent.Length];

                    for (int i = 0; i < results.Length; i++)
                    {
                        try
                        {
                            var scene = await _sceneService.LoadSceneAsync(recent[i]);
                            results[i] = new RecentFileViewModel(recent[i], scene.Name);
                        }
                        catch (FileNotFoundException ex)
                        {
                            results[i] = new RecentFileViewModel(recent[i], $"Could not locate file '{ex.FileName}'.", true);
                        }
                        catch (System.Text.Json.JsonException ex)
                        {
                            results[i] = new RecentFileViewModel(recent[i], $"Json format is invalid! Line: {ex.LineNumber}", true);
                        }
                        catch (Exception ex)
                        {
                            results[i] = new RecentFileViewModel(recent[i], ex.Message, true);
                        }
                    }

                    RecentItems = results;
                    _loadingRecents = false;
                });
            }
        }

        private void CheckProcesses()
        {
        }

        private void SetProcessList()
        {
            var processes = Process.GetProcessesByName("wallop");

            foreach (var proc in processes)
            {
                if(!OtherProcesses.Any(p => p.ProcessId == proc.Id))
                {
                    OtherProcesses.Add(new ProcessItemViewModel(proc.ProcessName, proc.Id));
                }
            }

            foreach (var listedProc in OtherProcesses)
            {
                if (!processes.Any(p => p.Id == listedProc.ProcessId))
                {
                    OtherProcesses.Remove(listedProc);
                }
            }

            this.RaisePropertyChanged(nameof(OtherProcesses));
        }

        private void SetSelectedFileByName()
        {
            if(string.IsNullOrEmpty(SelectedFile))
            {
                SelectedFile = SceneFileName;
                return;
            }

            string directory = "";
            if(Directory.Exists(SelectedFile))
            {
                directory = SelectedFile;
            }
            else
            {
                directory = new FileInfo(SelectedFile).Directory?.FullName ?? SelectedFile;
            }

            if (string.IsNullOrEmpty(directory))
            {
                SelectedFile = SceneFileName;
                return;
            }

            var filename = SceneFileName;
            if (string.IsNullOrEmpty(filename))
            {
                filename = Path.DirectorySeparatorChar.ToString();
            }
            SelectedFile = Path.Combine(directory, SceneFileName ?? filename);
        }

    }
}
