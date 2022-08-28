using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallop.Shared.ECS;

namespace WallopSceneEditor.Services
{
    public interface ISceneService
    {
        Task<StoredScene> LoadSceneAsync(string filePath);

        StoredScene CreateScene(string name, string filePath);
    }
}
