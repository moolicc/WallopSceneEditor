using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public partial class RealNumberControl : Avalonia.ReactiveUI.ReactiveUserControl<RealNumberViewModel>
    {
        public RealNumberControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
