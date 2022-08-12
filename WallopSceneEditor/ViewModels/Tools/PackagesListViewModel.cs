using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.ViewModels.Tools
{
    public class PackagesListViewModel : Tool
    {
        public ObservableCollection<ItemViewModel> Packages { get; set; }

        public PackagesListViewModel()
        {
            Packages = new ObservableCollection<ItemViewModel>();
        }
    }
}
