namespace WallopEdit.Presentation
{
    public partial record MainModel
    {
        private INavigator _navigator;

        public MainModel(
            INavigator navigator)
        {
            _navigator = navigator;
            Title = "Main";
        }

        public string? Title { get; }

        public IState<string> Name => State<string>.Value(this, () => string.Empty);

        public async Task GoToSecond()
        {
            var name = await Name;
            await _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(name!));
        }

    }
}