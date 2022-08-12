using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wallop.Shared.Modules;
using WallopSceneEditor.Services;

namespace WallopSceneEditor.ViewModels.Tools
{
    public class SceneTreeViewModel : Tool
    {
        public ObservableCollection<ItemViewModel> Modules { get; set; }


        public SceneTreeViewModel()
        {
            Modules = new ObservableCollection<ItemViewModel>();
        }
    }
}
