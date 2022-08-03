using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.ViewModels;

namespace WallopSceneEditor.Services
{
    internal class AvaloniaWindowService : IWindowService
    {
        public IClassicDesktopStyleApplicationLifetime Desktop { get; private set; }
        public Window MainWindow { get; init; }

        private MainWindowViewModel _mainWindowVm;
        private IScreen _mainWindowScreen;

        private IDependencyInjection _dependencyInjection;

        public AvaloniaWindowService(IClassicDesktopStyleApplicationLifetime desktop, IDependencyInjection dependencyInjection)
        {
            Desktop = desktop;
            MainWindow = Desktop.MainWindow;
            _mainWindowVm = (MainWindowViewModel)MainWindow.DataContext!;
            _mainWindowScreen = _mainWindowVm;
            _dependencyInjection = dependencyInjection;
        }

        public T ResolveView_Inject<T>() where T : ViewModelBase
        {
            return _dependencyInjection.CreateInstance<T>();
        }

        public T ResolveView<T>(params object[] args) where T : ViewModelBase
        {
            if(args == null || args.Length == 0)
            {
                return ResolveView_Inject<T>();
            }


            var viewModel = (T?)Activator.CreateInstance(typeof(T), args);

            if (viewModel == null)
            {
                throw new InvalidOperationException($"Failed to instantiate view {typeof(T).Name} with those args.");
            }

            return viewModel;
        }

        public T SwitchView<T>() where T : ViewModelBase
        {
            var view = ResolveView_Inject<T>();
            return SwitchView(view);
        }

        public T SwitchView<T>(T viewModel) where T : ViewModelBase
        {
            if(viewModel.HostScreen == null)
            {
                viewModel.HostScreen = _mainWindowScreen;
            }
            _mainWindowVm.Router.Navigate.Execute(viewModel);
            return viewModel;
        }

        public async Task ShowFileDialogAsync<TDialog>(Action<TDialog> with)
        {
            if (typeof(TDialog).IsAssignableFrom(typeof(Avalonia.Controls.SaveFileDialog)))
            {
                var dialog = Activator.CreateInstance<TDialog>();
                var sysDialog = dialog as Avalonia.Controls.SaveFileDialog;

                if(sysDialog == null)
                {
                    throw new InvalidOperationException();
                }

                with(dialog);

                await sysDialog.ShowAsync(MainWindow);
            }
            else if (typeof(TDialog).IsAssignableFrom(typeof(Avalonia.Controls.OpenFileDialog)))
            {
                var dialog = Activator.CreateInstance<TDialog>();
                var sysDialog = dialog as Avalonia.Controls.OpenFileDialog;

                if (sysDialog == null)
                {
                    throw new InvalidOperationException();
                }

                with(dialog);

                await sysDialog.ShowAsync(MainWindow);
            }
            else if (typeof(TDialog).IsAssignableFrom(typeof(Avalonia.Controls.OpenFolderDialog)))
            {
                var dialog = Activator.CreateInstance<TDialog>();
                var sysDialog = dialog as Avalonia.Controls.OpenFolderDialog;

                if (sysDialog == null)
                {
                    throw new InvalidOperationException();
                }

                with(dialog);

                await sysDialog.ShowAsync(MainWindow);
            }

            throw new InvalidCastException($"Invalid dialog type. Valid types are: {nameof(Avalonia.Controls.SaveFileDialog)}, {nameof(Avalonia.Controls.OpenFileDialog)}, and {nameof(Avalonia.Controls.OpenFolderDialog)}");
        }

        public void ScheduleOnUIThread(Action action)
        {
            Dispatcher.UIThread.Post(action, DispatcherPriority.Background);
        }
    }
}
