using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Plugins;

namespace WallopSceneEditor.ViewModels.Tools
{
    public class PropertyKeyValueViewModel : ViewModelBase, ISettingValue
    {
        public object? SettingCache { get; set; }
        public string Description { get; set; }

        public string Key { get; set; }
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                _valueChangedCallback(new KeyValuePair<string, string>(Key, value));
            }
        }
        public Control DisplayControl
        {
            get => _guiProvider.GetInlineDisplayControl(this, _settingArgs);
        }

        public Control EditControl
        {
            get => _guiProvider.GetInlineEditControl(this, _settingArgs);
        }

        public Control PopupEditControl
        {
            get => _guiProvider.GetPopoutEditControl(this, _settingArgs);
        }

        private string _value;
        private Action<KeyValuePair<string, string>> _valueChangedCallback;
        private ISettingTypeGuiProvider _guiProvider;
        private IEnumerable<KeyValuePair<string, string>>? _settingArgs;

        public PropertyKeyValueViewModel(string key, string value, string description, IEnumerable<KeyValuePair<string, string>>? keyValueArgs, ISettingTypeGuiProvider guiProvider, Action<KeyValuePair<string, string>> onValueChangedCallback)
        {
            Key = key;
            _value = value;
            Description = description;
            _settingArgs = keyValueArgs;
            _guiProvider = guiProvider;
            _valueChangedCallback = onValueChangedCallback;
        }
    }
}
