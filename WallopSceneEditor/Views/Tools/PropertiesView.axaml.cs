using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using WallopSceneEditor.Services;
using WallopSceneEditor.ViewModels.Tools;

namespace WallopSceneEditor.Views.Tools
{
    public partial class PropertiesView : ReactiveUserControl<PropertiesViewModel>
    {
        public PropertiesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SettingRevertClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var button = sender as Button;
            if(button == null)
            {
                return;
            }

            var vm = button.Tag as PropertySettingViewModel;
            if(vm == null)
            {
                return;
            }

            vm.RevertValue();
        }

        private async void SettingEditClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(ViewModel == null)
            {
                return;
            }
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            if (button.DataContext is PropertySettingViewModel propSetting && propSetting.GuiProvider.HandlesPopoutDialog)
            {
                await ViewModel.ShowSettingPopupDialog(propSetting).ConfigureAwait(false);
            }
            else if(sender is Control c)
            {
                FlyoutBase.ShowAttachedFlyout(c);
            }
        }
        
    }
}
