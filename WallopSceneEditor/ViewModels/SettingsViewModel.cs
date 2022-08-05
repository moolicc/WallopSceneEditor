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


        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand SceneFolderCommand { get; }
        public ICommand ModulesFolderCommand { get; }
        public ICommand PluginsFolderCommand { get; }


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

            OkCommand = ReactiveCommand.Create(async () =>
            {
                // TODO: Save settings
                var newAppSettings = new Models.AppSettingsModel()
                {
                    PackageDirectory = ModuleFolder,
                    SceneDirectory = SceneFolder,
                    PluginDirectory = PluginFolder
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
