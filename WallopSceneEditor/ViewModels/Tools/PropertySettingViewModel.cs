using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.ViewModels.Tools
{
    public class PropertySettingViewModel : ViewModelBase
    {
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
        public string Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }
        public bool Required
        {
            get => _required;
            set => this.RaiseAndSetIfChanged(ref _required, value);
        }


        private string _name;
        private string _value;
        private string _type;
        private bool _required;


        public PropertySettingViewModel(string name, string value, string type, bool required)
        {
            _name = name;
            _value = value;
            _type = type;
            _required = required;
        }
    }
}
