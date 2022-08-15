using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
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

                windowService.ShowNotification($"Director: {name}", "Director created without module.\nPlease assign a module to the new director.", NotificationTypes.Warning);
            });
            AddLayoutCommand = ReactiveCommand.Create(() =>
            {
                var name = FindName("New layout", s => _sceneMutator!.FindLayout(s) != null);
                sceneMutator.AddLayout(name);
                windowService.ShowNotification($"Layout: {name}", "New layout created.", NotificationTypes.Success);
            });
            AddActorCommand = ReactiveCommand.Create((string? modulePath) =>
            {
                if (ActiveLayout == null)
                {
                    _sceneMutator.AddLayout("New layout");
                }
                var name = FindName("New actor", s => _sceneMutator!.FindActor(ActiveLayout.NodeText, s) != null);
                sceneMutator.AddActor(ActiveLayout!.NodeText!, modulePath ?? "", name);

                windowService.ShowNotification($"Actor: {name}", "Actor created without module.\nPlease assign a module to the new actor.", NotificationTypes.Warning);

            });

            sceneMutator.OnDirectorAdded += SceneMutator_OnDirectorAdded;
            sceneMutator.OnLayoutAdded += SceneMutator_OnLayoutAdded;
            sceneMutator.OnActorAdded += SceneMutator_OnActorAdded;
            sceneMutator.OnValidateActor += SceneMutator_OnValidateActor;

            _sceneMutator = sceneMutator;
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

        public void RenameItem(ItemViewModel? viewModel, string oldName)
        {
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.Type == ItemTypes.Director)
            {
                if(_sceneMutator.FindDirector(viewModel.NodeText!) != null)
                {
                    _windowService.ShowNotification("Name conflict", "A director already exists with that name.", NotificationTypes.Error);
                    viewModel.NodeText = oldName;
                }
                else
                {
                    _sceneMutator.RenameDirector(oldName, viewModel.NodeText!);
                }
            }
            else if (viewModel.Type == ItemTypes.Layout)
            {
                if (_sceneMutator.FindLayout(viewModel.NodeText!) != null)
                {
                    _windowService.ShowNotification("Name conflict", "A layout already exists with that name.", NotificationTypes.Error);
                    viewModel.NodeText = oldName;
                }
                else
                {
                    _sceneMutator.RenameLayout(oldName, viewModel.NodeText!);
                }
            }
            else if (viewModel.Type == ItemTypes.Actor)
            {
                var parentLayout = ActiveLayout!.NodeText;
                if (_sceneMutator.FindActor(parentLayout, viewModel.NodeText!) != null)
                {
                    _windowService.ShowNotification("Name conflict", $"An actor already exists in the layout '{parentLayout}' with that name.", NotificationTypes.Error);
                    viewModel.NodeText = oldName;
                }
                else
                {
                    _sceneMutator.RenameActor(parentLayout, oldName, viewModel.NodeText!);
                }
            }
        }

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
                _windowService.ShowNotification($"Actor: {e.ActorName}", e.Messages[0], NotificationTypes.Error);
            }

            if (ActiveLayout == null)
            {
                _sceneMutator.AddLayout("New layout");
            }

            ActiveLayout!.AddChild(newActor);
            SelectAndExpandItem(newActor);
        }

        private void SceneMutator_OnValidateActor(object? sender, ActorEventArgs e)
        {
            var entry = FindActor(e.ParentLayout, e.ActorName);

            if(entry == null)
            {
                return;
            }

            entry.UpdateModulePath(e.ModulePath, e.ActorName);
            if (!e.OperationFailed)
            {
                if(entry.HasError)
                {
                    
                    _windowService.ShowNotification($"Actor: {e.ActorName}", "Actor validated successfully.", NotificationTypes.Success);
                }
                entry.ClearError();
            }
            else if (e.Messages.Count > 0)
            {
                entry.SetError(string.Join(' ', e.Messages));
                _windowService.ShowNotification($"Actor: {e.ActorName}", e.Messages[0], NotificationTypes.Error);
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
