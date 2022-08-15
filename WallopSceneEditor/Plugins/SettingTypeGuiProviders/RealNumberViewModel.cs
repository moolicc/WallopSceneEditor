using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public class RealNumberViewModel : ViewModels.ViewModelBase
    {
        public ISettingValue UnderlyingValue { get; set; }

        public double WorkingValue
        {
            get
            {
                if(double.TryParse(UnderlyingValue.Value, out var d))
                {
                    return d;
                }
                return 0;
            }
            set
            {
                UnderlyingValue.Value = value.ToString();
                this.RaisePropertyChanged(nameof(WorkingValue));
            }
        }

        public bool ReadOnly { get; set; }

        public RealNumberViewModel(ISettingValue underlyingVm, bool readOnly)
        {
            UnderlyingValue = underlyingVm;
            ReadOnly = readOnly;
        }
    }
}
