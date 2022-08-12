using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
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
    }
}
