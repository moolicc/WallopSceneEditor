using Dock.Model.ReactiveUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.ViewModels.Tools
{
    public class PropertiesViewModel : Tool
    {
        public ObservableCollection<PropertySettingViewModel> Properties { get; set; }

        public PropertiesViewModel()
        {
            Properties = new ObservableCollection<PropertySettingViewModel>()
            {
                new PropertySettingViewModel("name", "value", "string", true)
            };
        }
    }
}
