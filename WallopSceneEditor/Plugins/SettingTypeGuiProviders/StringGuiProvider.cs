﻿using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    /// <summary>
    /// Represents a string value.
    /// Accepts the following arguments:
    /// readonly=[true/false]
    /// length=[integer]
    /// </summary>
    public class StringGuiProvider : ISettingTypeGuiProvider
    {
        public string TypeName => "string";

        public bool HandlesPopoutDialog => false;


        public Control GetInlineDisplayControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var readOnly = true;
            var maxLength = SettingGuiProviderExtensions.GetValue<int?>(settingArgs, "length") ?? int.MaxValue;

            var vm = SettingGuiProviderExtensions.GetLinkedViewModel<StringViewModel>(value, nameof(StringViewModel.Text));
            vm.Readonly = readOnly;
            vm.MaxLength = maxLength;
            vm.UnderlyingValue = value;

            var control = new StringControl();

            control.DataContext = vm;

            return control;
        }

        public Control? GetInlineEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var readOnly = SettingGuiProviderExtensions.GetValue<bool?>(settingArgs, "readonly") ?? true;
            var maxLength = SettingGuiProviderExtensions.GetValue<int?>(settingArgs, "length") ?? int.MaxValue;

            var vm = SettingGuiProviderExtensions.GetLinkedViewModel<StringViewModel>(value, nameof(StringViewModel.Text));
            vm.Readonly = readOnly;
            vm.MaxLength = maxLength;
            vm.UnderlyingValue = value;

            var control = new StringControl();

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
