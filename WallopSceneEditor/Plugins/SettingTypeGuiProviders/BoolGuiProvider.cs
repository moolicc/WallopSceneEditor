using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    /// <summary>
    /// Represents a boolean value.
    /// Accepts the following arguments:
    /// readonly=[true/false]
    /// header="myheader"
    /// </summary>
    public class BoolGuiProvider : ISettingTypeGuiProvider
    {
        public string TypeName => "boolean";

        public bool HandlesPopoutDialog => false;



        public Control GetInlineDisplayControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var isReadonly = true;
            var header = SettingGuiProviderExtensions.GetValue<string>(settingArgs, "header") ?? "";

            var vm = SettingGuiProviderExtensions.GetLinkedViewModel<BoolViewModel>(value, nameof(BoolViewModel.SelectedValue));
            vm.Readonly = isReadonly;
            vm.Header = header;
            vm.UnderlyingValue = value;

            var control = new BoolControl();
            control.DataContext = vm;

            return control;
        }

        public Control? GetInlineEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var isReadonly = SettingGuiProviderExtensions.GetValue<bool?>(settingArgs, "readonly") ?? false;
            var header = SettingGuiProviderExtensions.GetValue<string>(settingArgs, "header") ?? "";

            var vm = SettingGuiProviderExtensions.GetLinkedViewModel<BoolViewModel>(value, nameof(BoolViewModel.SelectedValue));
            vm.Readonly = isReadonly;
            vm.Header = header;
            vm.UnderlyingValue = value;
            
            var control = new BoolControl();
            control.DataContext = vm;

            return control;
        }

        public Control? GetPopoutEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
            => GetInlineEditControl(value, settingArgs);

        public async Task<bool> OnShowPopoutDialogAsync(IWindowService windowService, ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return false;
        }
    }
}
