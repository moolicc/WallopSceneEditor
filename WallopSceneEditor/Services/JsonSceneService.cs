using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.Modules;
using Wallop.Shared.ECS;

namespace WallopSceneEditor.Services
{
    internal class JsonSceneService : ISceneService
    {
        public StoredScene CreateScene(string name, string filePath)
        {
            var storedScene = new StoredScene();
            storedScene.ConfigFile = filePath;
            storedScene.Name = name;
            return storedScene;
        }

        public async Task<StoredScene> LoadSceneAsync(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var result = await System.Text.Json.JsonSerializer.DeserializeAsync<StoredScene>(stream);
                if(result == null)
                {
                    throw new FileLoadException("Failed to load scene.");
                }
                result.ConfigFile = filePath;
                return result;
            }
        }
    }
}
