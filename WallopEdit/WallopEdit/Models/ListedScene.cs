using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.Models
{
    public partial record ListedScene
        (int ListedId, string SceneSource, string SceneName, string? SceneThumbnailFile, string? ErrorMessage)
    {
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    }
}
