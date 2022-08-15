using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using WallopSceneEditor.Services;
using WallopSceneEditor.ViewModels.Tools;

namespace WallopSceneEditor.Plugins
{
    public interface ISettingTypeGuiProvider
    {
        string TypeName { get; }
        bool HandlesPopoutDialog { get; }


        // TODO: Provide a way to inform the scene tree if this setting's current value is invalid.
        Control GetInlineDisplayControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs);
        Control? GetInlineEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs);


        Task<bool> OnShowPopoutDialogAsync(IWindowService windowService, ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs);
        Control? GetPopoutEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs);
    }
}
