using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    public partial class FileControl : UserControl
    {
        public FileControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
