using WallopEdit.Models;
using Windows.Foundation;

namespace WallopEdit.Presentation
{
    public partial record LandingModel(INavigator Navigator, Services.ISettingsService Config, Services.ISceneStorageService SceneStorage, Services.IEngineProcessService EngineProcs)
    {
        public IFeed<ListedScene[]> Scenes => Feed.Async(async _ =>
        {
            var result = await SceneStorage.ListScenesAsync().ToArrayAsync();
            return result;
        });
        public IListFeed<EngineSelection> EngineInstances => ListFeed.Async(async _ =>
        {
            List<EngineSelection> selections = new List<EngineSelection>
            {
                new EngineSelection(-1, "new")
            };
            selections.AddRange(await EngineProcs.GetEngineProcessesAsync().ToArrayAsync());

            return selections.ToImmutableList();
        });

        public IState<string> PreviewImage => State<string>.Empty(this);
        public IState<string> PreviewText => State<string>.Value(this, () => "Select a scene to preview.");
        public IState<EngineSelection> SelectedEngineProc => State<EngineSelection>.Value(this, () => new EngineSelection(-1, "new"));

        private ListedScene? _selectedScene;


        //public async Task GoToSecond()
        //{
        //    var name = await Name;
        //    await _navigator.NavigateViewModelAsync<EditorModel>(this, data: new Entity(name!));
        //}

        public async ValueTask UpdatePreviewImage(ListedScene selectedScene)
        {
            _selectedScene = selectedScene;

            await PreviewImage.Set(selectedScene.SceneThumbnailFile, default);
            if (selectedScene.SceneThumbnailFile == null)
            {
                await PreviewText.Set("Error loading scene preview.", default);
            }
            else
            {
                await PreviewText.Set("", default);
            }
        }

        public async Task GoToEdit()
        {
            await Navigator.NavigateViewModelAsync<EditorModel>(this);
        }

    }
}