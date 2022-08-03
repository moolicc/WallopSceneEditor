using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    internal class JsonSceneService : ISceneService
    {
        public Models.Scene LoadScene(string filePath, params string[] fields)
        {
            return new Models.Scene() { Name = System.IO.Path.GetFileNameWithoutExtension(filePath) };
        }
    }
}
