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

        public ICommand AddDirectorCommand { get; set; }
        public ICommand AddActorCommand { get; set; }

        public PackagesListViewModel(ISceneMutator sceneMutator, Func<string> getActiveLayoutCallback)
        {
            Packages = new ObservableCollection<ItemViewModel>();

            AddDirectorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                var name = "New director";
                if(modulePath != null)
                {
                    name = $"New {modulePath.Substring(modulePath.IndexOf('>') + 1)}";
                }

                sceneMutator.AddDirector(modulePath ?? "", name);
            });

            AddActorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                var name = "New director";
                if (modulePath != null)
                {
                    name = $"New {modulePath.Substring(modulePath.IndexOf('>') + 1)}";
                }

                var layout = getActiveLayoutCallback();
                sceneMutator.AddActor(layout, modulePath ?? "", name);
            });
        }
    }
}
