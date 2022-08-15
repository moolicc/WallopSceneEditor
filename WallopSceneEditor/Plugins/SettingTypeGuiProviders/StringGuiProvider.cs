using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    internal class StringGuiProvider : ISettingTypeGuiProvider
    {
        public string TypeName => "string";

        public bool HandlesPopoutDialog => false;

        public Control GetInlineDisplayControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            return new TextBlock() { Text = value.Value };
        }

        public Control? GetInlineEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            return null;
        }

        public Control? GetPopoutEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            return null;
        }

        public Task<bool> OnShowPopoutDialogAsync(IWindowService windowService, ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            throw new NotImplementedException();
        }
    }
}
