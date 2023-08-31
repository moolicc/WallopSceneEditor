using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopEdit.Models;

namespace WallopEdit.Services
{
    public interface ISceneStorageService
    {
        public IAsyncEnumerable<ListedScene> ListScenesAsync();
    }
}
