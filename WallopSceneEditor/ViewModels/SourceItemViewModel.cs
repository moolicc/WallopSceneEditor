using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WallopSceneEditor.ViewModels
{
    public class SourceItemViewModel : ViewModelBase
    {
        public Symbol? IconSymbol { get; set; }
        public string Header { get; set; } = "Header";
        public string Description { get; set; } = "Description";
        public ICommand ClickCommand { get; set; }
    }
}
