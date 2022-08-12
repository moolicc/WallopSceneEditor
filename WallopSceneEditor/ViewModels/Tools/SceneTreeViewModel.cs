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
        public ItemViewModel SceneTreeRoot
        {
            get => Modules.First();
            set => Modules = new ObservableCollection<ItemViewModel>(new[] { value });
        }

        public ItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ItemViewModel? ActiveLayout { get; set; }

        public ICommand AddDirectorCommand { get; set; }
        public ICommand AddLayoutCommand { get; set; }
        public ICommand AddActorCommand { get; set; }


        private ItemViewModel? _selectedItem;
        private ISceneMutator _sceneMutator;

        public SceneTreeViewModel(ISceneMutator sceneMutator)
        {
            Modules = new ObservableCollection<ItemViewModel>();

            AddDirectorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                sceneMutator.AddDirector(modulePath ?? "", "New director");
            });
            AddLayoutCommand = ReactiveCommand.Create(() =>
            {
                sceneMutator.AddLayout("New layout");
            });
            AddActorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                if (ActiveLayout == null)
                {
                    _sceneMutator.AddLayout("New layout");
                }
                sceneMutator.AddActor(ActiveLayout!.NodeText!, modulePath ?? "", "New actor");
            });

            sceneMutator.OnDirectorAdded += SceneMutator_OnDirectorAdded;
            sceneMutator.OnLayoutAdded += SceneMutator_OnLayoutAdded;
            sceneMutator.OnActorAdded += SceneMutator_OnActorAdded;

            _sceneMutator = sceneMutator;
        }

        public void RenameItem(ItemViewModel? viewModel, string oldName)
        {
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.Type == ItemTypes.Director)
            {
                _sceneMutator.RenameDirector(oldName, viewModel.NodeText!);
            }
            else if (viewModel.Type == ItemTypes.Layout)
            {
                _sceneMutator.RenameLayout(oldName, viewModel.NodeText!);
            }
            else if (viewModel.Type == ItemTypes.Actor)
            {
                _sceneMutator.RenameActor(ActiveLayout?.NodeText ?? "", oldName, viewModel.NodeText!);
            }
        }

        private void SceneMutator_OnDirectorAdded(object? sender, DirectorEventArgs e)
        {
            var newDir = ItemViewModel.CreateDirector(e.DirectorName, e.ModulePath, e.DirectorName);

            if (e.Messages.Count > 0)
            {
                newDir.SetError(string.Join(' ', e.Messages));
            }

            SceneTreeRoot.AddChild(newDir);
            SelectedItem = newDir;
        }

        private void SceneMutator_OnLayoutAdded(string layoutName)
        {
            var newLayout = ItemViewModel.CreateLayout(layoutName);
            ActiveLayout = newLayout;
            SceneTreeRoot.AddChild(newLayout);
            SelectedItem = newLayout;
        }

        private void SceneMutator_OnActorAdded(object? sender, ActorEventArgs e)
        {
            var newActor = ItemViewModel.CreateActor(e.ActorName, e.ModulePath, e.ActorName);

            if (e.Messages.Count > 0)
            {
                newActor.SetError(string.Join(' ', e.Messages));
            }

            if (ActiveLayout == null)
            {
                _sceneMutator.AddLayout("New layout");
            }

            ActiveLayout!.AddChild(newActor);
            SelectedItem = newActor;
        }
    }
}
