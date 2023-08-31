using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopEdit.Models;
using WallopEdit.Settings;

namespace WallopEdit.Services
{
    public class LocalSceneService : ISceneStorageService
    {
        public ApplicationSettings AppSettings { get; private set; }
        public LocalSceneService(ApplicationSettings settings)
        {
            AppSettings = settings;
        }

        public async IAsyncEnumerable<ListedScene> ListScenesAsync()
        {
            string[] files = Directory.GetFiles(AppSettings.SceneDirectory);
            for (int i = 0; i < files.Length; i++)
            {
                string? file = files[i];
                if (File.Exists(file))
                {
                    string contents = await File.ReadAllTextAsync(file);
                    yield return new ListedScene(i, file, contents);
                }
            }
        }
    }
}
