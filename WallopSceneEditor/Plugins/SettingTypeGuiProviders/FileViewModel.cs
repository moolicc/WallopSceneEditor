using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public class FileViewModel : ViewModels.ViewModelBase
    {
        public ISettingValue UnderlyingValue { get; set; }

        public string SelectedFile
        {
            get => _selectedFile;
            set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
        }
        private string _selectedFile;

        public FileViewModel(ISettingValue underlyingValue)
        {
            UnderlyingValue = underlyingValue;
            _selectedFile = "";
        }
    }
}
