using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using System.Collections.Generic;
using System.Globalization;
using System;
using WallopSceneEditor.ViewModels.Tools;
using Avalonia.Input;

namespace WallopSceneEditor.Views.Tools
{

    public partial class SceneTreeView : UserControl
    {
        public SceneTreeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
