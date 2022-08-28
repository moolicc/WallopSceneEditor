using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WallopSceneEditor.Views
{
    public partial class SettingsView : Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.SettingsViewModel>
    {
        public SettingsView()
        {
            InitializeComponent();

//            this.FindControl<Button>("OkButton").Click += (s, e) =>
//            {
//                Close(true);
//            };
//            this.FindControl<Button>("CancelButton").Click += (s, e) =>
//            {
//                Close(false);
//            };
//#if DEBUG
//            this.AttachDevTools();
//#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
