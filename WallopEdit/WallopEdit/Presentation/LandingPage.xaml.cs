using WallopEdit.Services;

namespace WallopEdit.Presentation
{
    public sealed partial class LandingPage : Page
    {
        public LandingPage()
        {
            this.InitializeComponent();
        }

        public LandingPage(BindableLandingModel model)
        {
            this.DataContext = model;
        }
    }
}