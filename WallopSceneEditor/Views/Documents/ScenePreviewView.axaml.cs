using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using WallopSceneEditor.ViewModels.Documents;

namespace WallopSceneEditor.Views.Documents
{
    public partial class ScenePreviewView : ReactiveUserControl<ScenePreviewViewModel>
    {
        public ScenePreviewView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
