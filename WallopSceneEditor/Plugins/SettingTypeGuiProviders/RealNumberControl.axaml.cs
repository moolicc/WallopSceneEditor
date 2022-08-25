using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;

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

        public void NumericUpDown_LostFocus(object? sender, RoutedEventArgs e)
        {
            ViewModel?.SetValue();
        }
    }
}
