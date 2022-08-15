using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.Modules.SettingTypes;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public class ChoiceGuiProvider : ISettingTypeGuiProvider
    {
        public string TypeName => "choice";

        public bool HandlesPopoutDialog => false;


        private static ChoiceControlViewModel GetViewModel(ISettingValue value, Option[] options)
        {
            if (value.SettingCache is not ChoiceControlViewModel)
            {
                var selected = options.FirstOrDefault(o => o.Value.Equals(value.Value, StringComparison.OrdinalIgnoreCase));

                value.SettingCache = new ChoiceControlViewModel(value, selected, options);
                value.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(ISettingValue.Value))
                    {
                        var val = sender as ISettingValue;
                        if (val == null)
                        {
                            return;
                        }

                        var vm = val.SettingCache as ChoiceControlViewModel;
                        if (vm == null)
                        {
                            return;
                        }
                        vm.RaisePropertyChanged(nameof(ChoiceControlViewModel.SelectedOption));
                    }
                };
            }
            return (ChoiceControlViewModel)value.SettingCache;
        }


        public Control GetInlineDisplayControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var choices = ChoiceType.GetOptions(settingArgs);

            var vm = GetViewModel(value, choices);
            vm.ReadOnly = true;

            var control = new ChoiceControl();

            control.DataContext = vm;
            return control;
        }

        public Control? GetInlineEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var choices = ChoiceType.GetOptions(settingArgs);

            var vm = GetViewModel(value, choices);
            vm.ReadOnly = false;

            var control = new ChoiceControl();

            control.DataContext = vm;
            return control;
        }

        public Control? GetPopoutEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            return GetInlineEditControl(value, settingArgs);
        }

        public async Task<bool> OnShowPopoutDialogAsync(IWindowService windowService, ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            await Task.CompletedTask;
            return false;
        }
    }
}
