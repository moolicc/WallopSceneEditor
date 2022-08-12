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


        private string _nodeText;
        private ItemToolTipViewModel _toolTip;


        public ItemViewModel(ItemTypes type, string tag, string nodeText, string icon, string description)
        {
            _nodeText = nodeText;

            Icon = icon;
            HasError = false;
            Description = description;

            _toolTip = new ItemToolTipViewModel(description, "module", "package");

            Children = new ObservableCollection<ItemViewModel>();
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

            return new ItemViewModel(ItemTypes.Setting, name, nodeText, "🛠", tooltip);
        }
    }
}
