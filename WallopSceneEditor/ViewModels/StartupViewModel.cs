using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
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


        public RecentFileViewModel(string filePath, string sceneName)
        {
            FilePath = filePath;
            SceneName = sceneName;
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


        private bool _updateSelectedByFileName = true;

        private ISettingsService _settingsService;
        private IWindowService _windowService;
        private ISceneService _sceneService;

        internal StartupViewModel(ISettingsService settingsService, IWindowService windowService, ISceneService sceneService)
        {
            SelectedFile = settingsService.GetSettingsAsync().Result.SceneDirectory;
            _settingsService = settingsService;
            _windowService = windowService;
            _sceneService = sceneService;
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
            if(item != null && item != RecentFileViewModel.Empty)
            {
                SceneName = item.SceneName;
                SelectedFile = item.FilePath;
            }
        }

        protected override void OnActivate()
        {
            _windowService.ScheduleOnUIThread(async () =>
            {
                var recent = (await _settingsService.GetRecentFilesAsync()).RecentFiles.ToArray();
                var results = new RecentFileViewModel[recent.Length];

                for (int i = 0; i < recent.Length; i++)
                {
                    var scene = _sceneService.LoadScene(recent[i], "");
                    results[i] = new RecentFileViewModel(recent[i], scene.Name);
                }

                RecentItems = results;
            });
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
