using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace WallopSceneEditor.Views
{
    public partial class SceneTreeItemView : Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.ItemViewModel>
    {
        public event Action<SceneTreeItemView, string, string>? OnItemRenamed;

        private string _textBeforeEdit;
        private TextBox _editBox; 

        public SceneTreeItemView()
        {
            InitializeComponent();

            _textBeforeEdit = "";

            this.LostFocus += SceneTreeItemView_LostFocus;
            this.FindControl<TextBlock>("NodeTextBlock").DoubleTapped += NameBlock_PointerPressed;
            _editBox = this.FindControl<TextBox>("NodeTextBox");
            _editBox.KeyDown += NameBox_KeyDown;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void NameBlock_PointerPressed(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel != null && !ViewModel.EditingText)
            {
                _editBox.Text = ViewModel.NodeText;
                _textBeforeEdit = ViewModel.NodeText;

                ViewModel.EditingText = true;
                _editBox.Focus();
                _editBox.SelectAll();
            }
        }

        private void SceneTreeItemView_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.EditingText)
            {
                EndTextEdit();
            }
        }

        private void NameBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Escape && ViewModel != null && ViewModel.EditingText)
            {
                ViewModel.EditingText = false;
            }
            else if (e.Key == Avalonia.Input.Key.Enter && ViewModel != null && ViewModel.EditingText)
            {
                EndTextEdit();
            }
        }

        private void EndTextEdit()
        {
            if(ViewModel == null)
            {
                return;
            }

            var newText = _editBox.Text;
            ViewModel.EditingText = false;
            OnItemRenamed?.Invoke(this, _textBeforeEdit, newText);
        }
    }
}
