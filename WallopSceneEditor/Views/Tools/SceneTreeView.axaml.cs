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
using Avalonia.VisualTree;
using Avalonia.Visuals;

namespace WallopSceneEditor.Views.Tools
{

    public partial class SceneTreeView : ReactiveUserControl<SceneTreeViewModel>
    {
        private TreeView _treeView;

        public SceneTreeView()
        {
            InitializeComponent();

            _treeView = this.FindControl<TreeView>("TreeView");
            _treeView.AutoScrollToSelectedItem = true;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if(ViewModel != null)
            {
                ViewModel.OnExpandToItem = ExpandTo;
            }
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

        private void OnItemRenamed(SceneTreeItemView item, string oldName, string newName)
        {
            ViewModel?.RenameItem(item.ViewModel, oldName, newName);
        }

        public void ExpandTo(ViewModels.ItemViewModel itemVm)
        {
            return;
            // TODO: Figure out a way to do this.
            var children = _treeView.GetVisualDescendants();

            foreach (var child in children)
            {
                if(child is TreeViewItem item)
                {
                    if(TryExpand(item, (ViewModels.ItemViewModel)item.DataContext, itemVm))
                    {
                        item.IsExpanded = true;
                    }
                }
            }
        }

        private bool TryExpand(TreeViewItem parent, ViewModels.ItemViewModel parentVm, ViewModels.ItemViewModel targetVm)
        {
            if(parentVm == targetVm)
            {
                return true;
            }
            var children = parent.GetVisualDescendants();

            foreach (var child in children)
            {
                if(child is SceneTreeItemView sceneTvItem && sceneTvItem.Parent is TreeViewItem tvItem)
                {
                    if(TryExpand(tvItem, sceneTvItem.ViewModel!, targetVm))
                    {
                        tvItem.IsExpanded = true;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
