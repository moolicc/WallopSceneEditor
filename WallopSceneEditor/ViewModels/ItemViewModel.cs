using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Views;

namespace WallopSceneEditor.ViewModels
{
    public enum ItemTypes
    {
        Package,
        Scene,
        Director,
        Layout,
        Actor,
        Setting
    }

    public class ItemViewModel : ViewModelBase
    {
        public ObservableCollection<ItemViewModel> Children { get; set; }

        public ItemTypes Type { get; set; }
        public string Tag { get; set; }
        public string Icon { get; set; }
        public bool HasError { get; private set; }

        public string Description { get; set; }

        public ItemToolTipViewModel ToolTip
        {
            get => _toolTip;
        }

        public string NodeText
        {
            get => _nodeText;
            set => this.RaiseAndSetIfChanged(ref _nodeText, value);
        }

        public bool EditingText
        {
            get => _editingText;
            set => this.RaiseAndSetIfChanged(ref _editingText, value);
        }

        public bool ItemSelected
        {
            get => _itemSelected;
            set => this.RaiseAndSetIfChanged(ref _itemSelected, value);
        }
        public bool Captioned { get; set; }

        public Action<ItemViewModel>? OnRenamedCallback;

        private string _nodeText;
        private bool _editingText;
        private bool _itemSelected;
        private ItemToolTipViewModel _toolTip;


        public ItemViewModel(ItemTypes type, string tag, string nodeText, string icon, string description)
        {
            _nodeText = nodeText;

            Type = type;
            Tag = tag;
            Icon = icon;
            HasError = false;
            Description = description;

            _toolTip = new ItemToolTipViewModel(description, "", "");
            if (type == ItemTypes.Actor || type == ItemTypes.Director)
            {
                SetToolTip(description, tag);
            }
            else
            {
                SetToolTip(description);
            }

            Children = new ObservableCollection<ItemViewModel>();
        }

        public void AddChild(ItemViewModel newChild)
        {
            Children.Add(newChild);
            this.RaisePropertyChanged(nameof(Children));
        }

        public void SetError(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                HasError = false;
                _toolTip.ErrorMessage = null;
                return;
            }

            _toolTip.ErrorMessage = errorMessage;
            HasError = true;
        }

        public void Rename(string newName)
        {
            OnRenamedCallback?.Invoke(this);
        }

        public void SetToolTip(string description)
        {
            _toolTip.Description = description;
        }

        public void SetToolTip(string description, string modulePath)
        {
            _toolTip.Description = description;
            if (modulePath != null)
            {
                if (modulePath.Contains('>'))
                {
                    var split = modulePath.Split(new[] { '>' }, 2);

                    _toolTip.Package = split[0];
                    _toolTip.Module = split[1];
                }
            }
        }

        public static ItemViewModel CreateScene(string sceneName, string sceneFile)
            => new ItemViewModel(ItemTypes.Scene, sceneFile, sceneName, "🎞", sceneFile);

        public static ItemViewModel CreateLayout(string layoutName)
            => new ItemViewModel(ItemTypes.Layout, "", layoutName, "📋", layoutName);


        public static ItemViewModel CreatePackage(string packageName, string packageFile)
            => new ItemViewModel(ItemTypes.Package, packageFile, packageName, "📦", packageFile);

        public static ItemViewModel CreateDirector(string directorName, string directorPath, string description)
            => new ItemViewModel(ItemTypes.Director, directorPath, directorName, "🎬", description);

        public static ItemViewModel CreateActor(string actorName, string actorPath, string description)
            => new ItemViewModel(ItemTypes.Actor, actorPath, actorName, "🎭", description);

        public static ItemViewModel CreateSetting(string name, string? value, string description, bool required)
        {
            var nodeText = name;
            if (value == null)
            {
                nodeText += $": {value}";
            }
            else if (string.IsNullOrWhiteSpace(value))
            {
                nodeText += $": \"{value}\"";
            }

            var tooltip = description;
            if (required)
            {
                tooltip += "\nRequired.";
            }
            else
            {
                tooltip += "\nOptional.";
            }

            return new ItemViewModel(ItemTypes.Setting, name, nodeText, "🛠", tooltip) { Captioned = true };
        }
    }

    /*
    public class SceneViewItemViewModel : ItemViewModel
    {

        public SceneViewItemViewModel(string sceneName, bool hasError, string tooltipText)
            : base(sceneName, "🎞 ", hasError, tooltipText)
        {
        }
    }

    public class LayoutViewItemViewModel : ItemViewModel
    {

        public LayoutViewItemViewModel(string layoutName, bool hasError, string tooltipText)
            : base(layoutName, "📋 ", hasError, tooltipText)
        {
        }
    }

    public class PackageViewItemViewModel : ItemViewModel
    {

        public PackageViewItemViewModel(string packageName, bool hasError, string tooltipText)
            : base(packageName, "📦 ", hasError, tooltipText)
        {
        }
    }

    public class DirectorModuleViewItemViewModel : ItemViewModel
    {
        public DirectorModuleViewItemViewModel(string directorName, bool hasError, string tooltipText)
            : base(directorName, "🎬 ", hasError, tooltipText)
        {
        }
    }

    public class ActorModuleViewItemViewModel : ItemViewModel
    {
        public ActorModuleViewItemViewModel(string actorName, bool hasError, string tooltipText)
            : base(actorName, "🎭 ", hasError, tooltipText)
        {
        }
    }

    public class SettingViewItemViewModel : ItemViewModel
    {
        public SettingViewItemViewModel(string settingName, string settingValue, string tooltipText)
            : base($"{settingName}: \"{settingValue}\"", "🛠 ", false, tooltipText)
        {
        }
    }*/
}
