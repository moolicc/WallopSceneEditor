using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WallopSceneEditor.Views
{
    public partial class PackageListItemView : Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.ItemViewModel>
    {
        public PackageListItemView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
