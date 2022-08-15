using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.Modules.SettingTypes;
using ReactiveUI;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public class ChoiceControlViewModel : ViewModels.ViewModelBase
    {
        public ISettingValue UnderlyingValue { get; set; }

        public bool ReadOnly
        {
            get => _readonly;
            set => this.RaiseAndSetIfChanged(ref _readonly, value);
        }

        public Option SelectedOption
        {
            get => _selectedOption;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedOption, value);
                UnderlyingValue.Value = value.Value;
            }
        }
        public Option[] Options => _options;

        private bool _readonly;
        private Option _selectedOption;
        private Option[] _options;

        public ChoiceControlViewModel(ISettingValue underlyingValue, Option selectedOption, Option[] options)
        {
            UnderlyingValue = underlyingValue;
            _selectedOption = selectedOption;
            _options = options;
        }
    }
}
