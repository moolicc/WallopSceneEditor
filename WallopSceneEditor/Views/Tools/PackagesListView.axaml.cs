using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using WallopSceneEditor.ViewModels.Tools;

namespace WallopSceneEditor.Views.Tools
{
    public partial class PackagesListView : ReactiveUserControl<PackagesListViewModel>
    {
        public PackagesListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
