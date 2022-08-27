using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using WallopSceneEditor.Services;
using WallopSceneEditor.ViewModels;
using WallopSceneEditor.Views;
using System;
using System.Diagnostics;
using WallopSceneEditor.Models;

namespace WallopSceneEditor
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {


                var mainWindow = new MainWindow();
                mainWindow.ViewModel = new MainWindowViewModel();

                desktop.MainWindow = mainWindow;

                var di = new HashedDependencyInjection();
                BuildServices(di);
                var windowService = new AvaloniaWindowService(desktop, di);
                di.Add<IWindowService, AvaloniaWindowService>(windowService);

                windowService.AddScreen("main", mainWindow.ViewModel);
                windowService.SwitchView<StartupViewModel>("main");


            }

            var now = DateTime.Now.Ticks;

            Debug.WriteLine("App started in: {0}", TimeSpan.FromTicks(now - Program.StartTime));
            base.OnFrameworkInitializationCompleted();
        }

        private void BuildServices(IDependencyInjection di)
        {
            di.Add<ISettingsService, JsonSettingsService>(new JsonSettingsService());
            di.Add<ISceneService, JsonSceneService>(new JsonSceneService());
            di.Add<IEngineService, WallopEngineService>(new WallopEngineService());
            di.Add<IPluginService, HardCodedPluginService>(new HardCodedPluginService());
            di.Add<ISessionSetupService, SessionSetupService>(new SessionSetupService());
        }
    }
}
