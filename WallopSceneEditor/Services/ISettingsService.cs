using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Models;

namespace WallopSceneEditor.Services
{
    public interface ISettingsService
    {
        Task<AppSettingsModel> GetSettingsAsync();

        Task<RecentFilesModel> GetRecentFilesAsync();

        Task SaveSettingsAsync(AppSettingsModel model);

        Task ClearRecentFilesAsync();
        Task AddRecentFileAsync(string file);
    }
}
