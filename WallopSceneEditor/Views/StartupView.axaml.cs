using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace WallopSceneEditor.Views
{
    public partial class StartupView : ReactiveUserControl<ViewModels.StartupViewModel>
    {
        public StartupView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnSelectedItemChanged(object sender, SelectionChangedEventArgs args)
        {
            if(args.AddedItems.Count <= 0)
            {
                ViewModel?.OnListSelectedItem(null);
            }
            else
            {
                var item = args.AddedItems[0] as ViewModels.RecentFileViewModel;
                ViewModel?.OnListSelectedItem(item);
            }
        }

        private void OtherProcsSelectedChanged(object? sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count <= 0)
            {
                ViewModel?.OnOtherProcessSelectedItem(null);
            }
            else
            {
                var item = args.AddedItems[0] as ViewModels.ProcessItemViewModel;
                ViewModel?.OnOtherProcessSelectedItem(item);
            }
        }
    }
}
