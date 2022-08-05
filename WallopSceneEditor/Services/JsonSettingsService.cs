using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using WallopSceneEditor.Models;
using System.Text.Json;

namespace WallopSceneEditor.Services
{
    internal class JsonSettingsService : ISettingsService
    {
        private const string RECENT_FILE = "recents.txt";
        private const string APP_FILE = "app.json";
        public async Task AddRecentFileAsync(string file)
        {
            using (FileStream fs = new FileStream(RECENT_FILE, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                await writer.WriteLineAsync(file);
            }
        }

        public Task ClearRecentFilesAsync()
        {
            if (!File.Exists(RECENT_FILE))
            {
                return Task.CompletedTask;
            }
            File.Delete(RECENT_FILE);

            return Task.CompletedTask;
        }

        public async Task<RecentFilesModel> GetRecentFilesAsync()
        {
            var result = new RecentFilesModel();
            if (!File.Exists(RECENT_FILE))
            {
                File.Create(RECENT_FILE);
                return result;
            }
            var fileContents = await File.ReadAllLinesAsync(RECENT_FILE);

            foreach (var item in fileContents)
            {
                result.RecentFiles.Add(item);
            }

            return result;
        }

        public async Task<AppSettingsModel> GetSettingsAsync()
        {
            if(!File.Exists(APP_FILE))
            {
                return new AppSettingsModel();
            }
            using (FileStream fs = new FileStream(APP_FILE, FileMode.OpenOrCreate, FileAccess.Read))
            {
                var result = JsonSerializer.Deserialize<AppSettingsModel>(fs);
                if(result == null)
                {
                    throw new FileLoadException("Failed to load settings file.");
                }
                return result;
            }
        }

        public async Task SaveSettingsAsync(AppSettingsModel model)
        {
            using (FileStream fs = new FileStream(APP_FILE, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(fs, model);
            }
        }
    }
}
