using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Models;

namespace WallopSceneEditor.Services
{
    internal class JsonSettingsService : ISettingsService
    {
        public async Task<Models.RecentFilesModel> GetRecentFilesAsync()
        {
            await Task.Delay(10000);
            return new RecentFilesModel(new[] { "test.json", "test2.json" });
        }

        public async Task<AppSettingsModel> GetSettingsAsync()
        {
            return new AppSettingsModel();
        }
    }
}
