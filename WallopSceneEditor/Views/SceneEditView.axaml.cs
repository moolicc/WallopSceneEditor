using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Avalonia.Win32;

namespace WallopSceneEditor.Views
{
    public partial class SceneEditView : Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.SceneEditViewModel>
    {
        public SceneEditView()
        {
            InitializeComponent();

        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
