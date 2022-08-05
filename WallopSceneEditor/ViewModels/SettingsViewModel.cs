using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public string SceneFolder { get => _sceneFolder; set => this.RaiseAndSetIfChanged(ref _sceneFolder, value); }
        private string _sceneFolder = "c:\\";

        public string ModuleFolder { get => _moduleFolder; set => this.RaiseAndSetIfChanged(ref _moduleFolder, value); }
        private string _moduleFolder = "c:\\";

        public string PluginFolder { get => _pluginFolder; set => this.RaiseAndSetIfChanged(ref _pluginFolder, value); }
        private string _pluginFolder = "c:\\";

        public string EnginePath { get => _enginePath; set => this.RaiseAndSetIfChanged(ref _enginePath, value); }
        private string _enginePath = "c:\\";

        public string EngineInstance { get => _engineInstance; set => this.RaiseAndSetIfChanged(ref _engineInstance, value); }
        private string _engineInstance = "";

        public int EngineWidth { get => _engineWidth; set => this.RaiseAndSetIfChanged(ref _engineWidth, value); }
        private int _engineWidth = 1;

        public int EngineHeight { get => _engineHeight; set => this.RaiseAndSetIfChanged(ref _engineHeight, value); }
        private int _engineHeight = 1;

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand SceneFolderCommand { get; }
        public ICommand ModulesFolderCommand { get; }
        public ICommand PluginsFolderCommand { get; }
        public ICommand EngineFileCommand { get; }


        private ISettingsService _settings;
        private IWindowService _windows;

        public SettingsViewModel(ISettingsService settings, IWindowService windows)
        {
            _settings = settings;
            _windows = windows;
            
            var appSettings = settings.GetSettingsAsync().Result;
            SceneFolder = appSettings.SceneDirectory;
            ModuleFolder = appSettings.PackageDirectory;
            PluginFolder = appSettings.PluginDirectory;

            EnginePath = appSettings.EnginePath;
            EngineInstance = appSettings.EngineConfig.InstanceName;
            EngineWidth = appSettings.EngineConfig.Width;
            EngineHeight = appSettings.EngineConfig.Height;

            OkCommand = ReactiveCommand.Create(async () =>
            {
                var newAppSettings = new Models.AppSettingsModel()
                {
                    PackageDirectory = ModuleFolder,
                    SceneDirectory = SceneFolder,
                    PluginDirectory = PluginFolder,
                    EnginePath = EnginePath,

                    EngineConfig = new Models.EngineConfigModel()
                    {
                        InstanceName = EngineInstance,
                        Width = EngineWidth,
                        Height = EngineHeight
                    }
                };

                await _settings.SaveSettingsAsync(newAppSettings);
            });

            CancelCommand = ReactiveCommand.Create(() =>
            {

            });


            SceneFolderCommand = ReactiveCommand.Create(async () =>
            {
                var folder = await _windows.ShowFileDialogAsync<OpenFolderDialog>(d => BuildFolderDialog(d, "Select Scene Folder", SceneFolder));
                if (folder != null)
                {
                    SceneFolder = folder;
                }
            });

            ModulesFolderCommand = ReactiveCommand.Create(async () =>
            {
                var folder = await _windows.ShowFileDialogAsync<OpenFolderDialog>(d => BuildFolderDialog(d, "Select Modules Folder", SceneFolder));
                if (folder != null)
                {
                    SceneFolder = folder;
                }
            });

            PluginsFolderCommand = ReactiveCommand.Create(async () =>
            {
                var folder = await _windows.ShowFileDialogAsync<OpenFolderDialog>(d => BuildFolderDialog(d, "Select Plugin Folder", SceneFolder));
                if (folder != null)
                {
                    SceneFolder = folder;
                }
            });

            EngineFileCommand = ReactiveCommand.Create(async () =>
            {
                var file = await _windows.ShowFileDialogAsync<OpenFileDialog>(
                    d =>
                    {
                        if(File.Exists(EnginePath))
                        {
                            d.Directory = new FileInfo(EnginePath).DirectoryName;
                            d.InitialFileName = Path.GetFileName(EnginePath);
                        }

                        d.AllowMultiple = false;
                        d.Title = "Select Engine Exe";
                        d.Filters.Add(new FileDialogFilter() { Name = "Executable files", Extensions = new List<string>(new[] { "exe" }) });
                    });

                if (file != null)
                {
                    EnginePath = file;
                }
            });
        }


        private void BuildFolderDialog(OpenFolderDialog dialog, string title, string defaultPath)
        {
            dialog.Title = title;

            if (!string.IsNullOrEmpty(defaultPath) && Directory.Exists(defaultPath))
            {
                dialog.Directory = defaultPath;
            }
        }
    }



}
