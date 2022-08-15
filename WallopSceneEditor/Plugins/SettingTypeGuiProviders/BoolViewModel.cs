using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.Modules.SettingTypes;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public class BoolViewModel : ViewModels.ViewModelBase
    {
        public ISettingValue UnderlyingValue { get; set; }

        public bool Readonly
        {
            get => _readonly;
            set => this.RaiseAndSetIfChanged(ref _readonly, value);
        }

        public bool SelectedValue
        {
            get
            {
                if (bool.TryParse(UnderlyingValue.Value, out var b))
                {
                    return b;
                }
                return false;
            }
            set
            {
                UnderlyingValue.Value = value.ToString();
                this.RaisePropertyChanged(nameof(SelectedValue));
            }
        }

        public string Header { get; set; }

        private bool _readonly;

        public BoolViewModel()
        {
        }
    }
}
