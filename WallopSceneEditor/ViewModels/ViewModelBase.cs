using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.ViewModels
{
    public class ViewModelBase : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        public ViewModelActivator Activator { get; }

        public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        public IScreen HostScreen { get; set;  }

        protected ViewModelBase()
        {
            Activator = new ViewModelActivator();
            this.WhenActivated(async (CompositeDisposable d) =>
            {
                OnActivate();
                await OnActivateAsync();
                Disposable.Create(() => OnDispose()).DisposeWith(d);
            });
        }

        protected virtual void OnActivate() { }
        protected virtual async Task OnActivateAsync() { }
        protected virtual void OnDispose() { }
    }
}
