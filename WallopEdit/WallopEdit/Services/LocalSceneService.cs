using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopEdit.Models;
using WallopEdit.SceneManagement;
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
                    string sceneName = "Error";
                    string? thumbnail = null;
                    string? errorMessage = null;

                    try
                    {
                        using (FileStream archiveStream = new FileStream(file, FileMode.Open))
                        {
                            ZipArchive archive = new ZipArchive(archiveStream);
                            sceneName = StoredSceneProtocol.Parts.GetName(archive);
                            thumbnail = StoredSceneProtocol.Parts.GetThumbnailFile(archive);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                    yield return new ListedScene(i, file, sceneName, thumbnail, errorMessage);
                }
            }
        }


    }
}
