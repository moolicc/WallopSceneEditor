using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using WallopSceneEditor.Services;
using WallopSceneEditor.ViewModels;
using WallopSceneEditor.Views;

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
                var di = new HashedDependencyInjection();
                BuildServices(di);


                desktop.MainWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel()
                };

                var windowService = new AvaloniaWindowService(desktop, di);
                di.Add<IWindowService, AvaloniaWindowService>(windowService);
                windowService.SwitchView<StartupViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void BuildServices(IDependencyInjection di)
        {
            di.Add<ISettingsService, JsonSettingsService>(new JsonSettingsService());
            di.Add<ISceneService, JsonSceneService>(new JsonSceneService());
        }
    }
}
