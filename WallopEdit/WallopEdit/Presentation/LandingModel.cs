namespace WallopEdit.Presentation
{
    public record SceneFile(string File);
    public partial record LandingModel(INavigator Navigator, Services.ISettingsService Config)
    {
        public IFeed<SceneFile[]> Scenes = Feed.Async(_ => ValueTask.FromResult(Directory.GetFiles(Config.AppSettings.SceneDirectory).Select(f => new SceneFile(f)).ToArray()));

        //public async Task GoToSecond()
        //{
        //    var name = await Name;
        //    await _navigator.NavigateViewModelAsync<EditorModel>(this, data: new Entity(name!));
        //}

        public async Task GoToEdit()
        {
            await Navigator.NavigateViewModelAsync<EditorModel>(this);
        }

    }
}