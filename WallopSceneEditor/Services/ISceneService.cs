using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    internal interface ISceneService
    {
        Models.Scene LoadScene(string filePath, params string[] fields);
    }
}
