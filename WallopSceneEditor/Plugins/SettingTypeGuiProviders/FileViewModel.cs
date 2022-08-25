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

        public string? SelectedFile
        {
            get => _selectedFile ?? ViewModels.Tools.PropertySettingViewModel.NIL_VALUE;
            set => this.RaiseAndSetIfChanged(ref _selectedFile, string.IsNullOrEmpty(value) ? null : value);
        }
        private string? _selectedFile;

        public FileViewModel(ISettingValue underlyingValue)
        {
            UnderlyingValue = underlyingValue;
            _selectedFile = "";
        }
    }
}
