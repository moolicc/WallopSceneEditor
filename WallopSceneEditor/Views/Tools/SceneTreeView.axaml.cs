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

    public partial class SceneTreeView : ReactiveUserControl<SceneTreeViewModel>
    {
        public SceneTreeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Tree_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null || e.AddedItems.Count != 1)
            {
                return;
            }

            var item = e.AddedItems[0] as ViewModels.ItemViewModel;
            if (item == null)
            {
                return;
            }

            if (item.Type == ViewModels.ItemTypes.Layout)
            {
                ViewModel.ActiveLayout = item;
            }
            else if (item.Type == ViewModels.ItemTypes.Actor)
            {
                // Find the item's parent.
                foreach (var layoutItem in ViewModel.SceneTreeRoot.Children)
                {
                    if (layoutItem.Type != ViewModels.ItemTypes.Layout)
                    {
                        continue;
                    }

                    bool found = false;
                    foreach (var child in layoutItem.Children)
                    {
                        if (child == item)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        ViewModel.ActiveLayout = layoutItem;
                        break;
                    }
                }
            }
            else
            {
                ViewModel.ActiveLayout = null;
            }
        }

        private void OnItemRenamed(SceneTreeItemView item, string oldName)
        {
            ViewModel?.RenameItem(item.ViewModel, oldName);
        }
    }
}
