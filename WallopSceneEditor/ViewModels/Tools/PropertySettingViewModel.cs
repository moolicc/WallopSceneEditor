using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;
using Wallop.Shared.Modules;
using WallopSceneEditor.Plugins;

namespace WallopSceneEditor.ViewModels.Tools
{
    public class PropertySettingViewModel : ViewModelBase, ISettingValue
    {
        public const string NIL_VALUE = "$nil";

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string? Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
                BoundSetting.Value = value ?? NIL_VALUE;
                _valueChangedCallback(new KeyValuePair<string, string?>(Name, value));
            }
        }
        public string Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }
        public bool Required
        {
            get => _required;
            set => this.RaiseAndSetIfChanged(ref _required, value);
        }
        public string Description { get; set; }

        public bool HasDefaultValue
        {
            get => _hasDefaultValue;
            set => this.RaiseAndSetIfChanged(ref _hasDefaultValue, value);
        }


        public Control DisplayControl
        {
            get => GuiProvider.GetInlineDisplayControl(this, SettingArgs);
        }

        public Control? EditControl
        {
            get => GuiProvider.GetInlineEditControl(this, SettingArgs) ?? DisplayControl;
        }

        public Control? PopupEditControl
        {
            get => GuiProvider.GetPopoutEditControl(this, SettingArgs) ?? DisplayControl;
        }

        public ISettingTypeGuiProvider GuiProvider { get; private set; }
        public object? SettingCache { get; set; }
        public IEnumerable<KeyValuePair<string, string?>>? SettingArgs { get; init; }

        public StoredSetting BoundSetting { get; init; }

        public string? InitialValue => _startingValue;

        private string _name;
        private string? _value;
        private string _type;
        private bool _required;
        private bool _hasDefaultValue;

        private string? _startingValue;
        private Action<KeyValuePair<string, string?>> _valueChangedCallback;



        public PropertySettingViewModel(StoredSetting boundSetting, IEnumerable<KeyValuePair<string, string>>? settingArgs, string name, string? initialValue, string description, string? value, string type, bool required, ISettingTypeGuiProvider guiProvider, Action<KeyValuePair<string, string?>> onValueChangedCallback)
        {
            _name = name;
            Description = description;
            _value = value;
            _type = type;
            _required = required;
            _startingValue = initialValue;
            GuiProvider = guiProvider;
            BoundSetting = boundSetting;
            SettingArgs = settingArgs;

            _valueChangedCallback = onValueChangedCallback;
        }
    }
}
