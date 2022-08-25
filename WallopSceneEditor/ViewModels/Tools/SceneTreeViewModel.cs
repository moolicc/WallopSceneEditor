using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Wallop.Shared.Modules;
using WallopSceneEditor.Models.EventData;
using WallopSceneEditor.Models.EventData.Mutator;
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
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedItem, value);
                SetNewMutatorContext();
            }
        }

        public ItemViewModel? ActiveLayout { get; set; }

        public ICommand AddDirectorCommand { get; set; }
        public ICommand AddLayoutCommand { get; set; }
        public ICommand AddActorCommand { get; set; }


        public Action<ItemViewModel>? OnExpandToItem;

        private ItemViewModel? _selectedItem;
        private ISceneMutator _sceneMutator;
        private IWindowService _windowService;

        public SceneTreeViewModel(ISceneMutator sceneMutator, IWindowService windowService)
        {
            _windowService = windowService;
            Modules = new ObservableCollection<ItemViewModel>();

            AddDirectorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                var name = FindName("New director", s => _sceneMutator!.FindDirector(s) != null);
                sceneMutator.AddDirector(modulePath ?? "", name);

                NotificationHelper.Notify(new Models.Message("Director", name, "Director created without module.\nPlease assign a module to the new director.", Models.MessageType.Warning));
            });
            AddLayoutCommand = ReactiveCommand.Create(() =>
            {
                var name = FindName("New layout", s => _sceneMutator!.FindLayout(s) != null);
                sceneMutator.AddLayout(name);

                NotificationHelper.Notify(new Models.Message("Layout", name, "Layout created with default parameters.\nPlease specify correct parameters.", Models.MessageType.Warning));
            });
            AddActorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                if (ActiveLayout == null)
                {
                    _sceneMutator.AddLayout("New layout");
                }
                var name = FindName("New actor", s => _sceneMutator!.FindActor(ActiveLayout.NodeText, s) != null);
                sceneMutator.AddActor(ActiveLayout!.NodeText!, modulePath ?? "", name);

                NotificationHelper.Notify(new Models.Message("Actor", name, "Actor created without module.\nPlease assign a module to the new actor.", Models.MessageType.Warning));

            });

            sceneMutator.OnValidatedLayout += SceneMutator_OnValidatedLayout;
            sceneMutator.OnDirectorAdded += SceneMutator_OnDirectorAdded;
            sceneMutator.OnLayoutAdded += SceneMutator_OnLayoutAdded;
            sceneMutator.OnActorAdded += SceneMutator_OnActorAdded;
            sceneMutator.OnValidateActor += SceneMutator_OnValidateActor;

            sceneMutator.OnDirectorRenamed += SceneMutator_OnDirectorRenamed;
            sceneMutator.OnLayoutRenamed += SceneMutator_OnLayoutRenamed;
            sceneMutator.OnActorRenamed += SceneMutator_OnActorRenamed;

            // TODO: We need to handle rename events. 
            // In the event of a layer rename, we need to re-associate all children with the new name.

            _sceneMutator = sceneMutator;
        }



        private void SceneMutator_OnDirectorRenamed(object? sender, MutatorValueChangedEventArgs<string> e)
        {
            if(!e.HasError)
            {
                var item = SceneTreeRoot.Children.FirstOrDefault(i => i.Type == ItemTypes.Director && i.NodeText! == e.OldValue)!;
                item.NodeText = e.NewValue;
                item.SetToolTip(e.NewValue);
            }
        }

        private void SceneMutator_OnLayoutRenamed(object? sender, MutatorValueChangedEventArgs<string> e)
        {
            if(!e.HasError)
            {
                var item = SceneTreeRoot.Children.FirstOrDefault(i => i.Type == ItemTypes.Layout && i.NodeText! == e.OldValue)!;
                item.NodeText = e.NewValue;
                item.SetToolTip(e.NewValue);
            }
        }

        private void SceneMutator_OnActorRenamed(object? sender, ActorValueChangedEventArgs<string> e)
        {
            if (!e.HasError)
            {
                var layout = SceneTreeRoot.Children.FirstOrDefault(i => i.Type == ItemTypes.Layout && i.NodeText! == e.ParentLayout);

                if(layout != null)
                {
                    var item = layout.Children.FirstOrDefault(i => i.Type == ItemTypes.Actor && i.NodeText! == e.OldValue)!;

                    item.NodeText = e.NewValue;
                    item.SetToolTip(e.NewValue);
                }

            }
        }

        public string GetOrCreateActiveLayout()
        {
            if (ActiveLayout == null)
            {
                _sceneMutator.AddLayout("New layout");
                return "New layout";
            }
            return ActiveLayout.NodeText;
        }

        public void RenameItem(ItemViewModel? viewModel, string oldName, string newName)
        {
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.Type == ItemTypes.Director)
            {
                _sceneMutator.RenameDirector(oldName, newName);
            }
            else if (viewModel.Type == ItemTypes.Layout)
            {
                _sceneMutator.RenameLayout(oldName, newName);
            }
            else if (viewModel.Type == ItemTypes.Actor)
            {
                var parentLayout = ActiveLayout!.NodeText;
                _sceneMutator.RenameActor(parentLayout, oldName, newName);
            }
        }

        // Note: This function is an exact replica of the function of the same name in the PackagesListViewModel.
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
                if(!number.HasValue)
                {
                    number = 0;
                }
                number++;
            }

            return Cat(baseName, number);
        }

        private void SceneMutator_OnDirectorAdded(object? sender, DirectorEventArgs e)
        {
            var newDir = ItemViewModel.CreateDirector(e.DirectorName, e.ModulePath, e.DirectorName);

            if (e.HasError)
            {
                newDir.SetError(string.Join(' ', e.Messages.Select(m => m.Text)));
            }

            SceneTreeRoot.AddChild(newDir);
            SelectedItem = newDir;
        }

        private void SceneMutator_OnLayoutAdded(object? sender, LayoutEventArgs e)
        {
            var newLayout = ItemViewModel.CreateLayout(e.LayoutName);
            ActiveLayout = newLayout;

            if (e.HasError)
            {
                newLayout.SetError(string.Join(' ', e.Messages.Select(m => m.Text)));
                NotificationHelper.Notify(e.Messages[0]);
            }

            SceneTreeRoot.AddChild(newLayout);
            SelectedItem = newLayout;
        }

        private void SceneMutator_OnActorAdded(object? sender, ActorEventArgs e)
        {
            var newActor = ItemViewModel.CreateActor(e.ActorName, e.ModulePath, e.ActorName);

            if (e.HasError)
            {
                newActor.SetError(string.Join(' ', e.Messages.Select(m => m.Text)));
                NotificationHelper.Notify(e.Messages[0]);
            }

            if (ActiveLayout == null)
            {
                _sceneMutator.AddLayout("New layout");
            }

            ActiveLayout!.AddChild(newActor);
            SelectAndExpandItem(newActor);
        }

        private void SceneMutator_OnValidatedLayout(object? sender, LayoutEventArgs e)
        {
            var entry = SceneTreeRoot.Children.FirstOrDefault(c => c.NodeText == e.LayoutName);

            if (entry == null)
            {
                return;
            }

            entry.UpdateModulePath("", e.LayoutName);
            if (!e.HasError)
            {
                if (entry.HasError)
                {
                    NotificationHelper.Notify(new Models.Message("Layout", e.LayoutName, "Layout validated successfully.", Models.MessageType.Success));
                }
                entry.ClearError();
            }
            else
            {
                entry.SetError(string.Join(' ', e.Messages.Select(m => m.Text)));
                NotificationHelper.Notify(e.Messages[0]);
            }
        }

        private void SceneMutator_OnValidateActor(object? sender, ActorEventArgs e)
        {
            var entry = FindActor(e.ParentLayout, e.ActorName);

            if(entry == null)
            {
                return;
            }

            entry.UpdateModulePath(e.ModulePath, e.ActorName);
            if (!e.HasError)
            {
                if(entry.HasError)
                {
                    NotificationHelper.Notify(new Models.Message("Actor", e.ActorName, "Actor validated successfully.", Models.MessageType.Success));
                }
                entry.ClearError();
            }
            else
            {
                entry.SetError(string.Join(' ', e.Messages.Select(m => m.Text)));
                NotificationHelper.Notify(e.Messages[0]);
            }
        }

        private ItemViewModel? FindActor(string parentLayout, string actorName)
        {
            ItemViewModel? Recurse(ItemViewModel parent, string parentLayout, string actorName)
            {
                foreach (var item in parent.Children)
                {
                    if(parent.NodeText == parentLayout && item.NodeText == actorName)
                    {
                        return item;
                    }

                    var found = Recurse(item, parentLayout, actorName);
                    if(found != null)
                    {
                        return found;
                    }
                }
                return null;
            }

            return Recurse(SceneTreeRoot, parentLayout, actorName);
        }

        private void SetNewMutatorContext()
        {
            if(SelectedItem == null)
            {
                _sceneMutator.PropertyContext = null;
                return;
            }

            if(SelectedItem.Type == ItemTypes.Actor)
            {
                _sceneMutator.SetPropertyContextToActor(ActiveLayout?.NodeText, SelectedItem.NodeText);
            }
            else if(SelectedItem.Type == ItemTypes.Director)
            {
                _sceneMutator.SetPropertyContextToDirector(SelectedItem.NodeText);
            }
            else if(SelectedItem.Type == ItemTypes.Scene)
            {
                _sceneMutator.SetPropertyContextToScene(SelectedItem.NodeText);
            }
            else if(SelectedItem.Type == ItemTypes.Layout)
            {
                _sceneMutator.SetPropertyContextToLayout(SelectedItem.NodeText);
            }
            else
            {
                _sceneMutator.ClearPropertyContext();
            }
        }

        private void SelectAndExpandItem(ItemViewModel selected)
        {
            SelectedItem = selected;
            OnExpandToItem?.Invoke(selected);
        }

    }
}
