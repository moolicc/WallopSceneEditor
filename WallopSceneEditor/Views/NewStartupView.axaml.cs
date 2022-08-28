using Avalonia.Controls;
using Avalonia.ReactiveUI;
using WallopSceneEditor.Services;
using WallopSceneEditor.ViewModels;

namespace WallopSceneEditor.Views
{
    public partial class NewStartupView : ReactiveUserControl<NewStartupViewModel>
    {
        public NewStartupView()
        {
            InitializeComponent();
        }

        private void SettingsClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            FluentAvalonia.UI.Controls.ContentDialog d = new FluentAvalonia.UI.Controls.ContentDialog();

            ViewModel?.ShowSettings();
        }
    }
}
