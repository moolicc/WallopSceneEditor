﻿using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Models;
using WallopSceneEditor.ViewModels;

namespace WallopSceneEditor.Services
{
    internal class AvaloniaWindowService : IWindowService
    {
        public IClassicDesktopStyleApplicationLifetime Desktop { get; private set; }
        public Window MainWindow { get; init; }
        public IntPtr WindowHandle => MainWindow.PlatformImpl.Handle.Handle;

        public Dictionary<string, IScreen> Screens { get; init; }


        private IDependencyInjection _dependencyInjection;
        private Stack<Window> _dialogStack;

        public AvaloniaWindowService(IClassicDesktopStyleApplicationLifetime desktop, IDependencyInjection dependencyInjection)
        {
            Desktop = desktop;
            MainWindow = desktop.MainWindow;
            Screens = new Dictionary<string, IScreen>();
            _dependencyInjection = dependencyInjection;

            _dialogStack = new Stack<Window>();
            _dialogStack.Push(MainWindow);
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

        public T SwitchView<T>(string screen) where T : ViewModelBase
        {
            var view = ResolveView_Inject<T>();
            return SwitchView(screen, view);
        }

        public T SwitchView<T>(string screen, T viewModel) where T : ViewModelBase
        {
            var targetScreen = Screens[screen];

            if(viewModel.HostScreen == null)
            {
                viewModel.HostScreen = targetScreen;
            }

            if(viewModel is SceneEditViewModel edit)
            {
                var factory = new MainDockFactory(edit.SessionData, edit.SessionMutator);
                var layout = factory.CreateLayout();
                factory.InitLayout(layout);

                edit.Factory = factory;
                edit.Layout = layout;
            }

            targetScreen.Router.Navigate.Execute(viewModel);
            return viewModel;
        }

        public async Task<string?> ShowFileDialogAsync<TDialog>(Action<TDialog>? with = null)
        {
            if (typeof(TDialog).IsAssignableFrom(typeof(SaveFileDialog)))
            {
                var dialog = Activator.CreateInstance<TDialog>();
                var sysDialog = dialog as SaveFileDialog;

                if(sysDialog == null)
                {
                    throw new InvalidOperationException();
                }

                with?.Invoke(dialog);

                return await sysDialog.ShowAsync(_dialogStack.Peek());
            }
            else if (typeof(TDialog).IsAssignableFrom(typeof(OpenFileDialog)))
            {
                var dialog = Activator.CreateInstance<TDialog>();
                var sysDialog = dialog as OpenFileDialog;

                if (sysDialog == null)
                {
                    throw new InvalidOperationException();
                }

                with?.Invoke(dialog);

                return (await sysDialog.ShowAsync(_dialogStack.Peek()))?[0];
            }
            else if (typeof(TDialog).IsAssignableFrom(typeof(OpenFolderDialog)))
            {
                var dialog = Activator.CreateInstance<TDialog>();
                var sysDialog = dialog as OpenFolderDialog;

                if (sysDialog == null)
                {
                    throw new InvalidOperationException();
                }

                with?.Invoke(dialog);

                return await sysDialog.ShowAsync(_dialogStack.Peek());
            }

            throw new InvalidCastException($"Invalid dialog type. Valid types are: {nameof(Avalonia.Controls.SaveFileDialog)}, {nameof(OpenFileDialog)}, and {nameof(Avalonia.Controls.OpenFolderDialog)}");
        }

        public void ScheduleOnUIThread(Action action)
        {
            Dispatcher.UIThread.Post(action, DispatcherPriority.Background);
        }

        public void AddScreen<TScreen>(string screenName, TScreen screen) where TScreen : IScreen
        {
            Screens.Add(screenName, screen);
        }

        public IScreen GetScreen(string screenName)
        {
            return Screens[screenName];
        }

        public async Task<TResult> ShowDialogAsync<TDialog, TResult>(TDialog dialog)
        {
            if(dialog is Window win)
            {
                var parent = _dialogStack.Peek();

                _dialogStack.Push(win);
                var result = await win.ShowDialog<TResult>(parent);

                _dialogStack.Pop();

                return result;
            }
            throw new InvalidCastException("Invalid type specified.");
        }
    }
}
