using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.ViewModels;

namespace WallopSceneEditor.Services
{
    internal interface IWindowService
    {
        T ResolveView_Inject<T>() where T : ViewModelBase;
        
        T ResolveView<T>(params object[] args) where T : ViewModelBase;

        T SwitchView<T>() where T : ViewModelBase;
        T SwitchView<T>(T viewModel) where T : ViewModelBase;

        Task ShowFileDialogAsync<TDialog>(Action<TDialog> dialog);

        void ScheduleOnUIThread(Action action);
    }
}
