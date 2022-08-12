using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace WallopSceneEditor.Views
{
    public partial class SceneTreeItemView : Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.ItemViewModel>
    {
        public SceneTreeItemView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
