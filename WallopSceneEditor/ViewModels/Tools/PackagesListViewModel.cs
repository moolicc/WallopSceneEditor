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

        public PackagesListViewModel(ISceneMutator sceneMutator, Func<string> getOrCreateActiveLayoutCallback)
        {
            Packages = new ObservableCollection<ItemViewModel>();

            AddDirectorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                var name = "New director";
                if(modulePath != null)
                {
                    name = $"New {modulePath.Substring(modulePath.IndexOf('>') + 1)}";
                }

                name = FindName(name, s => sceneMutator.FindDirector(s) != null);
                sceneMutator.AddDirector(modulePath ?? "", name);
            });

            AddActorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                var name = "New actor";
                if (modulePath != null)
                {
                    name = $"New {modulePath.Substring(modulePath.IndexOf('>') + 1)}";
                }

                var layout = getOrCreateActiveLayoutCallback();
                name = FindName(name, s => sceneMutator.FindActor(layout, s) != null);

                sceneMutator.AddActor(layout, modulePath ?? "", name);
            });
        }

        // Note: This function is an exact replica of the function of the same name in the SceneTreeViewModel.
        private string FindName(string baseName, Func<string, bool> exists)
        {
            string Cat(string name, int? number)
            {
                if (!number.HasValue)
                    return name;
                return $"{name} {number.Value}";
            }

            int? number = null;

            while (exists(Cat(baseName, number)))
            {
                if (!number.HasValue)
                {
                    number = 0;
                }
                number++;
            }

            return Cat(baseName, number);
        }
    }
}
