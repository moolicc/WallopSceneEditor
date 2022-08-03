using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor
{
    internal class AppViewLocator : IViewLocator
    {
        public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
        {
            var name = viewModel!.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name)!;

            IViewFor control = (IViewFor)Activator.CreateInstance(type)!;
            control.ViewModel = viewModel;
            return control;
        }
    }
}
