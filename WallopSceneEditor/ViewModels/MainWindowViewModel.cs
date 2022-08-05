using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace WallopSceneEditor.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {

        public RoutingState Router { get; } = new RoutingState();
    }
}
