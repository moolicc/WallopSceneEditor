using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    internal interface ISettingsService
    {
        Task<Models.AppSettingsModel> GetSettingsAsync();

        Task<Models.RecentFilesModel> GetRecentFilesAsync();
    }
}
