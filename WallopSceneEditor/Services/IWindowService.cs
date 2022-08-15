using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.ViewModels;

namespace WallopSceneEditor.Services
{
    public enum NotificationTypes
    {
        Information = 0,
        Success = 1,
        Warning = 2,
        Error = 3
    }

    public interface IWindowService
    {
        IntPtr WindowHandle { get; }
        T ResolveView_Inject<T>() where T : ViewModelBase;
        T ResolveView<T>(params object[] args) where T : ViewModelBase;

        void AddScreen<TScreen>(string screenName, TScreen screen) where TScreen : IScreen;
        IScreen GetScreen(string screenName);

        T SwitchView<T>(string screen) where T : ViewModelBase;
        T SwitchView<T>(string screen, T viewModel) where T : ViewModelBase;

        Task<string?> ShowFileDialogAsync<TDialog>(Action<TDialog>? dialog = null);

        Task<TResult> ShowDialogAsync<TDialog, TResult>(TDialog dialog);

        void ScheduleOnUIThread(Action action);

        void ShowNotification(string title, string text, NotificationTypes notificationType = NotificationTypes.Information, Action? onClick = null, Action? onClose = null);
    }
}
