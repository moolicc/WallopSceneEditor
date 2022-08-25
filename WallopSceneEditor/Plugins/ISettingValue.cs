using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.ViewModels.Tools;

namespace WallopSceneEditor.Plugins
{
    public interface ISettingValue : INotifyPropertyChanged
    {
        string? Value { get; set; }
        object? SettingCache { get; set; }
    }
}
