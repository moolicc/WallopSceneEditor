using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public class StringViewModel : ViewModels.ViewModelBase
    {
        public ISettingValue UnderlyingValue { get; set; }

        public string Text
        {
            get
            {
                return UnderlyingValue.Value;
            }
            set
            {
                UnderlyingValue.Value = value;
                this.RaisePropertyChanged(nameof(Text));
            }
        }

        public int MaxLength { get; set; }

        public bool Readonly
        {
            get => _readOnly;
            set
            {
                if(_readOnly != value)
                {
                    _readOnly = value;
                    this.RaisePropertyChanged(nameof(Text));
                }
            }
        }

        private bool _readOnly;

        public StringViewModel()
        {
        }
    }
}
