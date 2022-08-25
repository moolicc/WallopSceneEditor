using Dock.Model.ReactiveUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WallopSceneEditor.Plugins.SettingTypeGuiProviders;
using WallopSceneEditor.Services;
using Wallop.Shared.ECS;
using WallopSceneEditor.Models;
using Wallop.Shared.Modules;
using DynamicData;
using Wallop.Shared.Modules.SettingTypes;
using WallopSceneEditor.Models.EventData;
using WallopSceneEditor.Models.EventData.Mutator;
using Avalonia.Threading;

namespace WallopSceneEditor.ViewModels.Tools
{
    public class PropertiesViewModel : Tool
    {
        // TODO: Make this configurable
        public const int APPLY_TIME = 5;

        public ObservableCollection<PropertySettingViewModel> Settings { get; set; }
        public ObservableCollection<PropertyKeyValueViewModel> KeyValues { get; set; }

        public PropertySettingViewModel? SelectedSetting
        {
            get => _selectedSetting;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedSetting, value);
                this.RaisePropertyChanged(nameof(SelectedDescription));
            }
        }

        public PropertyKeyValueViewModel? SelectedKeyValue
        {
            get => _selectedKeyValue;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedKeyValue, value);
                this.RaisePropertyChanged(nameof(SelectedDescription));
            }
        }


        private PropertySettingViewModel? _selectedSetting;
        private PropertyKeyValueViewModel? _selectedKeyValue;


        public bool ModuleGridVisible
        {
            get => _moduleGridVisible;
            set => this.RaiseAndSetIfChanged(ref _moduleGridVisible, value);
        }

        public bool KeyValueGridVisible
        {
            get => _keyValueGridVisible;
            set => this.RaiseAndSetIfChanged(ref _keyValueGridVisible, value);
        }

        public string SelectedDescription
        {
            get
            {
                if(ModuleGridVisible)
                {
                    return _selectedSetting?.Description ?? "";
                }
                else if(KeyValueGridVisible)
                {
                    return _selectedKeyValue?.Description ?? "";
                }
                return "";
            }
        }

        private bool _moduleGridVisible = true;
        private bool _keyValueGridVisible = false;
        private ISceneMutator _mutator;
        private SessionDataModel _dataModel;
        private IPluginService _plugins;
        private IWindowService _windowService;

        private List<KeyValuePair<string, string>> _actorChoices;
        private List<KeyValuePair<string, string>> _directorChoices;

        private DispatcherTimer _applyTimer;
        private string? _applySetting;


        public PropertiesViewModel(ISceneMutator sceneMutator, IPluginService plugins, IWindowService windowService, SessionDataModel model)
        {
            _mutator = sceneMutator;
            _mutator.OnPropertyContextChanged += mutator_OnPropertyContextChanged;
            _plugins = plugins;
            _windowService = windowService;
            _dataModel = model;

            Settings = new ObservableCollection<PropertySettingViewModel>();
            KeyValues = new ObservableCollection<PropertyKeyValueViewModel>();


            _applyTimer = new DispatcherTimer();
            _applyTimer.Tick += (_, _) =>
                {
                    _applySetting = null;
                    Validate();
                };
            _applyTimer.Interval = TimeSpan.FromSeconds(APPLY_TIME);
            _applySetting = null;

            _actorChoices = new List<KeyValuePair<string, string>>();
            _directorChoices = new List<KeyValuePair<string, string>>();

            foreach (var pkg in model.Packages)
            {
                foreach (var mod in pkg.DeclaredModules)
                {
                    var modPath = pkg.Info.ManifestPath + ">" + mod.ModuleInfo.ScriptName;
                    var label = mod.ModuleInfo.ScriptName;

                    if (mod.ModuleInfo.ScriptType == ModuleTypes.Actor)
                    {
                        _actorChoices.Add(new KeyValuePair<string, string>("option", $"{label}{ChoiceType.LABEL_VALUE_DELIMITER}{mod.ModuleInfo.Id}"));
                    }
                    else
                    {
                        _directorChoices.Add(new KeyValuePair<string, string>("option", $"{label}{ChoiceType.LABEL_VALUE_DELIMITER}{mod.ModuleInfo.Id}"));
                    }
                }
            }
        }

        private void mutator_OnPropertyContextChanged(object? sender, MutatorValueChangedEventArgs<object?> e)
        {
            // Note: This will cause memory leaks since there may be events subscribed or something from within a guiprovider's dealings.
            Settings.Clear();
            KeyValues.Clear();


            if(_applySetting != null)
            {
                _applySetting = null;
                Validate();
            }
            _applyTimer.Stop();


            if (_mutator.PropertyContext is ActorContext actor)
            {
                var module = _dataModel.FindModule(actor.RelatedModule);
                if(module == null)
                {
                    SetupUnkownModuleContext(actor.RelatedModule, true);
                    return;
                }

                Settings.AddRange(GetCombinedSettingsFor(actor.RelatedModule, module));
                KeyValueGridVisible = false;
                ModuleGridVisible = true;

                this.RaisePropertyChanged(nameof(Settings));
            }
            else if (_mutator.PropertyContext is DirectorContext director)
            {
                var module = _dataModel.FindModule(director.RelatedModule);
                if (module == null)
                {
                    SetupUnkownModuleContext(director.RelatedModule, false);
                    return;
                }

                Settings.AddRange(GetCombinedSettingsFor(director.RelatedModule, module));
                KeyValueGridVisible = false;
                ModuleGridVisible = true;

                this.RaisePropertyChanged(nameof(Settings));
            }
            else if(_mutator.PropertyContext is LayoutContext layout)
            {
                var ro = new KeyValuePair<string, string>("readonly", "false");

                var nameVm = new PropertyKeyValueViewModel("Name", layout.Layout.Name, $"The layout's name.", new[] { ro }, new StringGuiProvider(), (kvp) =>
                {
                    _mutator.RenameLayout(layout.Layout.Name, kvp.Value);
                    OnSettingChanged(new KeyValuePair<string, string?>(kvp.Key, kvp.Value));
                });
                var activeVm = new PropertyKeyValueViewModel("Active", layout.Layout.Active.ToString(), $"Whether or not the layout is active.", null, new BoolGuiProvider(), (kvp) =>
                {
                    layout.Layout.Active = bool.Parse(kvp.Value);
                    OnSettingChanged(new KeyValuePair<string, string?>(kvp.Key, kvp.Value));
                });

                var screenChoiceSettings = new List<KeyValuePair<string, string>>();
                var screens = (_windowService as AvaloniaWindowService)!.MainWindow.Screens;

                screenChoiceSettings.Add(new KeyValuePair<string, string>("option", "Extended|0"));

                for (int i = 0; i < screens.ScreenCount; i++)
                {
                    if (screens.All[i].Primary)
                    {
                        screenChoiceSettings.Insert(1, new KeyValuePair<string, string>("option", $"Primary|{i + 1}"));
                    }
                    else
                    {
                        screenChoiceSettings.Add(new KeyValuePair<string, string>("option", $"Display {i}|{i + 1}"));
                    }
                }

                var screenVm = new PropertyKeyValueViewModel("Screen", layout.Layout.ScreenIndex.ToString(), $"The screen to display this layout on.", screenChoiceSettings, new ChoiceGuiProvider(), (kvp) =>
                {
                    layout.Layout.ScreenIndex = int.Parse(kvp.Value);
                    OnSettingChanged(new KeyValuePair<string, string?>(kvp.Key, kvp.Value));
                });


                var renderWidthVm = new PropertyKeyValueViewModel("Resolution width", layout.Layout.RenderWidth.ToString(), $"The width of the layout's resolution.", null, new RealNumberGuiProvider(), (kvp) =>
                {
                    layout.Layout.RenderWidth = int.Parse(kvp.Value);
                    OnSettingChanged(new KeyValuePair<string, string?>(kvp.Key, kvp.Value));
                });
                var renderHeightVm = new PropertyKeyValueViewModel("Resolution height", layout.Layout.RenderHeight.ToString(), $"The actual height of the layout.", null, new RealNumberGuiProvider(), (kvp) =>
                {
                    layout.Layout.RenderHeight = int.Parse(kvp.Value);
                    OnSettingChanged(new KeyValuePair<string, string?>(kvp.Key, kvp.Value));
                });

                KeyValues.Add(nameVm);
                KeyValues.Add(activeVm);
                KeyValues.Add(screenVm);
                KeyValues.Add(renderWidthVm);
                KeyValues.Add(renderHeightVm);
                ModuleGridVisible = false;
                KeyValueGridVisible = true;

                this.RaisePropertyChanged(nameof(KeyValues));
            }
            else
            {
                Settings = new ObservableCollection<PropertySettingViewModel>();
                this.RaisePropertyChanged(nameof(Settings));
            }
        }

        private void SetupUnkownModuleContext(StoredModule stored, bool isActor)
        {
            var choices = _actorChoices;
            var typeString = "actor";
            if(!isActor)
            {
                choices = _directorChoices;
                typeString = "director";
            }

            var kvVm = new PropertyKeyValueViewModel("Module", "", $"The {typeString}'s backing module.", choices, new ChoiceGuiProvider(), (kvp) =>
            {
                stored.ModuleId = kvp.Value;
                _mutator.ValidatePropertyContextAsModule();
            });

            KeyValues.Add(kvVm);
            ModuleGridVisible = false;
            KeyValueGridVisible = true;

            this.RaisePropertyChanged(nameof(KeyValues));
            return;
        }

        private IEnumerable<PropertySettingViewModel> GetCombinedSettingsFor(StoredModule stored, Module declared)
        {
            var guiProviders = _plugins.GetImplementations<Plugins.ISettingTypeGuiProvider>();

            var result = new List<PropertySettingViewModel>();
            foreach (var setting in declared.ModuleSettings)
            {
                var storedSetting = stored.Settings.FirstOrDefault(s => s.Name == setting.SettingName);

                var curValue = storedSetting?.Value;
                if(storedSetting == null)
                {
                    curValue = setting.DefaultValue;
                    storedSetting = new StoredSetting(setting.SettingName, curValue);
                    stored.Settings.Add(storedSetting);
                }

                // TODO: Don't hardcode the realnumberguiprovider.
                var provider = guiProviders.FirstOrDefault(g => g.TypeName == setting.SettingType);

                if(provider == null)
                {
                    _windowService.ShowNotification($"Loaded module: {stored.ModuleId}", $"Cannot find associated Gui Provider for specified setting type.\nSetting: {setting.SettingName}\nType: {setting.SettingType}", NotificationTypes.Error);
                    provider = new StringGuiProvider();
                }

                var vm = new PropertySettingViewModel(storedSetting, setting.SettingTypeArgs, setting.SettingName, setting.DefaultValue, setting.SettingDescription,
                    curValue, setting.SettingType,
                    setting.Required, provider, OnSettingChanged);

                result.Add(vm);
            }

            foreach (var setting in stored.Settings)
            {
                if(declared.ModuleSettings.Any(s => s.SettingName == setting.Name))
                {
                    continue;
                }

                // TODO: Allow user to specify setting type, description.
                var vm = new PropertySettingViewModel(setting, null, setting.Name, setting.Value, "User added value",
                    setting.Value, "string", false, new StringGuiProvider(), OnSettingChanged);
                result.Add(vm);
            }

            return result;
        }

        public bool DoesSelectedTypeHandlePopup()
        {
            if(_moduleGridVisible && _selectedSetting != null)
            {
                return _selectedSetting.GuiProvider.HandlesPopoutDialog;
            }
            return false;
        }

        public async Task ShowSettingPopupDialog(PropertySettingViewModel vm)
        {
            var currentValue = vm.Value;
            if (await vm.GuiProvider.OnShowPopoutDialogAsync(_windowService, vm, vm.SettingArgs).ConfigureAwait(false))
            {
                //Validate();
            }
            else
            {
                vm.Value = currentValue;
            }
        }

        public void RevertValue(PropertySettingViewModel vm)
        {
            vm.Value = vm.InitialValue;
            Validate();
        }

        private void OnSettingChanged(KeyValuePair<string, string?> setting)
        {
            _applyTimer.Stop();
            _applyTimer.Start();
            if (_applySetting != setting.Key)
            {
                if(_applySetting != null)
                {
                    Validate();
                }
                _applySetting = setting.Key;
            }
        }

        public void Validate()
        {
            if (_moduleGridVisible)
            {
                _applyTimer.Stop();
                _applySetting = null;
                _mutator.ValidatePropertyContextAsModule();
            }
            else if(_keyValueGridVisible)
            {
                _applyTimer.Stop();
                _applySetting = null;
                _mutator.ValidatePropertyContextAsLayout();
            }
        }
    }
}
