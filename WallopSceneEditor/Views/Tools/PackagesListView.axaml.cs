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

        private void Item_DoubleClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var item = sender as PackageListItemView;
            if (ViewModel == null || item == null || item.ViewModel == null || item.ViewModel.Type == ViewModels.ItemTypes.Package)
            {
                return;
            }

            if (item.ViewModel.Type == ViewModels.ItemTypes.Actor)
            {
                ViewModel.AddActorCommand.Execute($"{item.ViewModel.Tag}");
            }
            else if (item.ViewModel.Type == ViewModels.ItemTypes.Director)
            {
                ViewModel.AddDirectorCommand.Execute($"{item.ViewModel.Tag}");
            }
        }

        public void ItemPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var item = sender as Control;
            if (item == null || item.Tag == null)
            {
                return;
            }

            var data = item.Tag.ToString();
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            var dragData = new DataObject();
            dragData.Set(DataFormats.Text, data);
        }
    }
}
