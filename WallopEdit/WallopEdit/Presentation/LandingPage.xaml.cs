using Microsoft.UI.Xaml.Media.Animation;
using WallopEdit.Models;
using WallopEdit.Services;

namespace WallopEdit.Presentation
{
    public sealed partial class LandingPage : Page
    {
        private const int ENGINE_TIMER_MILLIS = 10000;

        private DispatcherTimer _engineTimer;

        public LandingPage()
        {
            this.InitializeComponent();
            _engineTimer = new DispatcherTimer();
            _engineTimer.Interval = TimeSpan.FromMilliseconds(ENGINE_TIMER_MILLIS);
            _engineTimer.Tick += EngineTimer_Tick;
            _engineTimer.Start();
        }

        private void EngineTimer_Tick(object? sender, object e)
        {
            EnginesFeedView.Refresh.Execute(null);
        }

        public LandingPage(BindableLandingModel model)
        {
            this.DataContext = model;

        }

        private void OnSelectedSceneChanges(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var context = (BindableLandingModel)DataContext;
            var viewItem = args.SelectedItem;

            if(viewItem is ListedScene scene)
            {
                context.UpdatePreviewImage.Execute(scene);
            }
        }
    }
}