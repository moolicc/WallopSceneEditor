using Avalonia.Data;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;
using Wallop.Shared.Modules;
using WallopSceneEditor.Models;
using WallopSceneEditor.Models.Documents;
using WallopSceneEditor.Models.Tools;
using WallopSceneEditor.Services;
using WallopSceneEditor.ViewModels;
using WallopSceneEditor.ViewModels.Documents;
using WallopSceneEditor.ViewModels.Tools;

namespace WallopSceneEditor
{
    public class MainDockFactory : Factory
    {
        private IDocumentDock _documentDock;
        private readonly SessionDataModel _context;
        private readonly ISceneMutator _sceneMutator;

        public MainDockFactory(SessionDataModel dataModel, ISceneMutator mutator)
        {
            _context = dataModel;
            _sceneMutator = mutator;
        }

        public override IRootDock CreateLayout()
        {
            var sceneDocument = new ScenePreviewViewModel
            {
                Id = "Scene",
                Title = _context.LoadedScene.Name,
                CanClose = false
            };




            var sceneTree = new SceneTreeViewModel()
            {
                Id = "SceneTree",
                Title = "Scene Tree",
                Modules = new System.Collections.ObjectModel.ObservableCollection<ItemViewModel>(new[] { BuildSceneTree(_context.LoadedScene) })
            };

            var packagesList = new PackagesListViewModel
            {
                Id = "PackagesList",
                Title = "Packages",
                Packages = new System.Collections.ObjectModel.ObservableCollection<ItemViewModel>(BuildPackageTree(_context.Packages))
            };


            var propertiesTable = new PropertiesViewModel
            {
                Id = "PropertiesTable",
                Title = "Properties"
            };

            var output = new OutputViewModel
            {
                Id = "Output",
                Title = "Output"
            };

            var documentDock = new DocumentDock
            {
                Id = "DocumentsPane",
                Title = "DocumentsPane",
                Proportion = double.NaN,
                ActiveDockable = sceneDocument,
                CanClose = false,
                CanCreateDocument = false,
                VisibleDockables = CreateList<IDockable>
                (
                    sceneDocument
                )
            };


            //documentDock.CanCreateDocument = false;
            //documentDock.CreateDocument = ReactiveCommand.Create(() =>
            //{
            //    var index = documentDock.VisibleDockables?.Count + 1;
            //    var document = new TestDocumentViewModel { Id = $"Document{index}", Title = $"Document{index}", CanClose = false };
            //    this.AddDockable(documentDock, document);
            //    this.SetActiveDockable(document);
            //    this.SetFocusedDockable(documentDock, document);
            //});

            var mainLayout = new ProportionalDock
            {
                Id = "MainLayout",
                Title = "MainLayout",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    new ProportionalDock
                    {
                        Id = "LeftPane",
                        Title = "LeftPane",
                        Proportion = 0.25,
                        Orientation = Orientation.Vertical,
                        ActiveDockable = null,
                        VisibleDockables = CreateList<IDockable>
                        (
                            new ToolDock
                            {
                                Id = "LeftPaneTop",
                                Title = "LeftPaneTop",
                                Proportion = double.NaN,
                                ActiveDockable = packagesList,
                                VisibleDockables = CreateList<IDockable>
                                (
                                    sceneTree
                                ),
                                Alignment = Alignment.Left,
                                GripMode = GripMode.Visible
                            },
                            new ProportionalDockSplitter()
                            {
                                Id = "LeftPaneTopSplitter",
                                Title = "LeftPaneTopSplitter"
                            },
                            new ToolDock
                            {
                                Id = "LeftPaneBottom",
                                Title = "LeftPaneBottom",
                                Proportion = double.NaN,
                                ActiveDockable = packagesList,
                                VisibleDockables = CreateList<IDockable>
                                (
                                    packagesList,
                                    propertiesTable
                                ),
                                Alignment = Alignment.Left,
                                GripMode = GripMode.Visible
                            }
                        )
                    },
                    new ProportionalDockSplitter()
                    {
                        Id = "LeftSplitter",
                        Title = "LeftSplitter"
                    },
                    new ProportionalDock
                    {
                        Id = "Center",
                        Title = "Center",
                        Proportion = double.NaN,
                        Orientation = Orientation.Vertical,

                        VisibleDockables = CreateList<IDockable>
                        (
                            documentDock,
                            new ProportionalDockSplitter()
                            {
                                Id = "CenterSplitter",
                                Title = "CenterSplitter"
                            },
                            new ToolDock
                            {
                                Id = "CenterBottom",
                                Title = "CenterBottom",
                                Proportion = 0.25,
                                ActiveDockable = output,
                                VisibleDockables = CreateList<IDockable>
                                (
                                    output
                                ),
                                Alignment = Alignment.Bottom,
                                GripMode = GripMode.Visible
                            }
                        )
                    }
                )
            };

            var mainView = new MainDockViewModel
            {
                Id = "Main",
                Title = "Main",
                ActiveDockable = mainLayout,
                VisibleDockables = CreateList<IDockable>(mainLayout)
            };

            var root = CreateRootDock();

            root.Id = "Root";
            root.Title = "Root";
            root.ActiveDockable = mainView;
            root.DefaultDockable = mainView;
            root.VisibleDockables = CreateList<IDockable>(mainView);

            _documentDock = documentDock;

            return root;
        }

        public override void InitLayout(IDockable layout)
        {
            this.ContextLocator = new Dictionary<string, Func<object>>
            {
                [nameof(IRootDock)] = () => _context,
                [nameof(IProportionalDock)] = () => _context,
                [nameof(IDocumentDock)] = () => _context,
                [nameof(IToolDock)] = () => _context,
                [nameof(IProportionalDockSplitter)] = () => _context,
                [nameof(IDockWindow)] = () => _context,
                [nameof(IDocument)] = () => _context,
                [nameof(ITool)] = () => _context,
                ["Scene"] = () => new ScenePreviewModel(),
                ["PackagesList"] = () => new PackagesListModel(),
                ["SceneTree"] = () => new SceneTreeModel(),
                ["PropertiesTable"] = () => new PropertiesModel(),
                ["Output"] = () => new OutputModel(),
                ["LeftPaneTopSplitter"] = () => _context,
                ["LeftPaneBottom"] = () => _context,
                ["RightPane"] = () => _context,
                ["RightPaneTop"] = () => _context,
                ["RightPaneTopSplitter"] = () => _context,
                ["RightPaneBottom"] = () => _context,
                ["DocumentsPane"] = () => _context,
                ["Center"] = () => _context,
                ["CenterSplitter"] = () => _context,
                ["CenterBottom"] = () => _context,
                ["MainLayout"] = () => _context,
                ["LeftSplitter"] = () => _context,
                ["RightSplitter"] = () => _context,
                ["MainLayout"] = () => _context,
                ["Main"] = () => _context,
            };

            this.HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () =>
                {
                    var hostWindow = new HostWindow()
                    {
                        [!HostWindow.TitleProperty] = new Binding("ActiveDockable.Title")
                    };
                    return hostWindow;
                }
            };

            this.DockableLocator = new Dictionary<string, Func<IDockable>>
            {
            };

            base.InitLayout(layout);

            this.SetActiveDockable(_documentDock);
            this.SetFocusedDockable(_documentDock, _documentDock.VisibleDockables?.FirstOrDefault());
        }

        private ItemViewModel[] BuildPackageTree(IEnumerable<Package> packages)
        {
            var packageItems = new List<ItemViewModel>();

            foreach (var pkg in packages)
            {
                var item = ItemViewModel.CreatePackage(pkg.Info.PackageName, pkg.Info.ManifestPath);

                foreach (var mod in pkg.DeclaredModules)
                {
                    var modPath = $"{mod.ModuleInfo.PackageFile}>{mod.ModuleInfo.ScriptName}";
                    ItemViewModel modItem;
                    if (mod.ModuleInfo.ScriptType == ModuleTypes.Actor)
                    {
                        modItem = ItemViewModel.CreateActor(mod.ModuleInfo.ScriptName, modPath, mod.ModuleInfo.ScriptDescription);
                    }
                    else
                    {
                        modItem = ItemViewModel.CreateDirector(mod.ModuleInfo.ScriptName, modPath, mod.ModuleInfo.ScriptDescription);
                    }

                    foreach (var set in mod.ModuleSettings)
                    {
                        var setItem = ItemViewModel.CreateSetting(set.SettingName, set.DefaultValue, set.SettingDescription, set.Required);

                        modItem.Children.Add(setItem);
                    }

                    item.Children.Add(modItem);
                }

                packageItems.Add(item);
            }

            return packageItems.ToArray();
        }

        private ItemViewModel BuildSceneTree(StoredScene scene)
        {
            var root = ItemViewModel.CreateScene(scene.Name, scene.ConfigFile ?? "New scene file");

            foreach (var dir in scene.DirectorModules)
            {
                var dirNode = ItemViewModel.CreateDirector(dir.InstanceName, dir.ModuleId, dir.ModuleId);

                foreach (var set in dir.Settings)
                {
                    var setNode = ItemViewModel.CreateSetting(set.Name, set.Value, "TODO", false);
                    dirNode.Children.Add(setNode);
                }

                root.Children.Add(dirNode);
            }

            foreach (var layout in scene.Layouts)
            {
                var layoutNode = ItemViewModel.CreateLayout(layout.Name);


                foreach (var actor in layout.ActorModules)
                {
                    var actorNode = ItemViewModel.CreateActor(actor.InstanceName, actor.ModuleId, actor.ModuleId);

                    foreach (var set in actor.Settings)
                    {
                        var setNode = ItemViewModel.CreateSetting(set.Name, set.Value, "TODO", false);
                        actorNode.Children.Add(setNode);
                    }

                    root.Children.Add(actorNode);
                }

                root.Children.Add(layoutNode);
            }

            return root;
        }
    }
}
