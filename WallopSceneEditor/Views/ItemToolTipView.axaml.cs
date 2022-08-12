using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WallopSceneEditor.Views
{
    public partial class ItemToolTipView : UserControl //Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.ItemToolTipViewModel>
    {
        public ItemToolTipView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
