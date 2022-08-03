using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models
{
    internal class RecentFilesModel
    {
        public List<string> RecentFiles { get; private set; }

        public RecentFilesModel()
        {
            RecentFiles = new List<string>();
        }

        public RecentFilesModel(IEnumerable<string> intialSet)
        {
            RecentFiles = new List<string>(intialSet);
        }
    }
}
