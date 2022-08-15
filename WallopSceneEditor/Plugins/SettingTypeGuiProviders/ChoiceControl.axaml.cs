using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public partial class ChoiceControl : UserControl
    {
        public ChoiceControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
