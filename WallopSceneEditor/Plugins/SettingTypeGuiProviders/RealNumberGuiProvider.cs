using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using RealPrecision = Wallop.Shared.Modules.SettingTypes.RealNumberPrecision;
using RealType = Wallop.Shared.Modules.SettingTypes.RealNumberType;
using Avalonia.Data;
using System.Reactive.Subjects;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public class RealNumberGuiProvider : ISettingTypeGuiProvider
    {
        public string TypeName => "real";

        public bool HandlesPopoutDialog => false;

        private static RealNumberViewModel GetViewModel(ISettingValue value)
        {
            if (value.SettingCache is not RealNumberViewModel)
            {
                value.SettingCache = new RealNumberViewModel(value, true);
                value.PropertyChanged += (sender, e) =>
                {
                    if(e.PropertyName == nameof(ISettingValue.Value))
                    {
                        var val = sender as ISettingValue;
                        if(val == null)
                        {
                            return;
                        }

                        var vm = val.SettingCache as RealNumberViewModel;
                        if(vm == null)
                        {
                            return;
                        }
                        vm.RaisePropertyChanged(nameof(RealNumberViewModel.WorkingValue));
                    }
                };
            }
            return (RealNumberViewModel)value.SettingCache;
        }

        public Control GetInlineDisplayControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var vm = GetViewModel(value);
            vm.ReadOnly = true;

            var control = new RealNumberControl();

            control.ViewModel = vm;
            return control;
        }

        public Control? GetInlineEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var vm = GetViewModel(value);
            vm.ReadOnly = false;

            var control = new RealNumberControl();

            control.ViewModel = vm;
            return control;
        }

        public Control? GetPopoutEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var control = GetInlineEditControl(value, settingArgs)!;
            control.MinWidth = 200;
            return control;
        }

        public async Task<bool> OnShowPopoutDialogAsync(IWindowService windowService, ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            await Task.CompletedTask;
            return false;
        }
    }
}
