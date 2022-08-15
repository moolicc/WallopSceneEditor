using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace WallopSceneEditor.Views
{
    public partial class SceneTreeItemView : Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.ItemViewModel>
    {
        public event Action<SceneTreeItemView, string>? OnItemRenamed;

        private string _textBeforeEdit;

        public SceneTreeItemView()
        {
            InitializeComponent();

            _textBeforeEdit = "";

            this.LostFocus += SceneTreeItemView_LostFocus;
            this.FindControl<TextBlock>("NodeTextBlock").DoubleTapped += NameBlock_PointerPressed;
            this.FindControl<TextBox>("NodeTextBox").KeyDown += NameBox_KeyDown;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void NameBlock_PointerPressed(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel != null && !ViewModel.EditingText)
            {
                ViewModel.EditingText = true;

                var textbox = this.FindControl<TextBox>("NodeTextBox");
                _textBeforeEdit = textbox.Text;
                textbox.Focus();
                textbox.SelectAll();
            }
        }

        private void SceneTreeItemView_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.EditingText = false;
                ViewModel.SetToolTip(ViewModel.NodeText);
                OnItemRenamed?.Invoke(this, _textBeforeEdit);
            }
        }

        private void NameBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Escape && ViewModel != null && ViewModel.EditingText)
            {
                ViewModel.NodeText = _textBeforeEdit;
                ViewModel.EditingText = false;
            }
            else if (e.Key == Avalonia.Input.Key.Enter && ViewModel != null && ViewModel.EditingText)
            {
                ViewModel.EditingText = false;
                ViewModel.SetToolTip(ViewModel.NodeText);
                OnItemRenamed?.Invoke(this, _textBeforeEdit);
            }
        }
    }
}
