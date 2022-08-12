using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.ViewModels
{
    public class ItemToolTipViewModel : ViewModelBase
    {
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public bool ShowError
        {
            get => !string.IsNullOrEmpty(_errorMessage);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                this.RaiseAndSetIfChanged(ref _errorMessage, value);
                this.RaisePropertyChanged(nameof(ShowError));
            }
        }

        public string Module
        {
            get => _module;
            set => this.RaiseAndSetIfChanged(ref _module, value);
        }

        public string Package
        {
            get => _package;
            set => this.RaiseAndSetIfChanged(ref _package, value);
        }


        private string _description;
        private string? _errorMessage;
        private string _module;
        private string _package;

        public ItemToolTipViewModel(string description, string module, string package)
        {
            _errorMessage = null;
            _description = description;
            _module = module;
            _package = package;
        }
    }
}
