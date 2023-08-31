using WallopEdit.Models;

namespace WallopEdit.Presentation
{
    public partial record LandingModel(INavigator Navigator, Services.ISettingsService Config, Services.ISceneStorageService SceneStorage)
    {
        public IFeed<ListedScene[]> Scenes => Feed.Async(async _ =>
        {
            var result = await SceneStorage.ListScenesAsync().ToArrayAsync();
            return result;
        });

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