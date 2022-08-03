using Avalonia.Controls;
using Avalonia.ReactiveUI;

namespace WallopSceneEditor.Views
{
    public partial class MainWindow : ReactiveWindow<ViewModels.MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
