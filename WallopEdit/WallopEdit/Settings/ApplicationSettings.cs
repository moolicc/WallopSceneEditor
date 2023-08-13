using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.Settings
{
    /// <summary>
    /// Represents settings related to WallopEdit as an isolated application.
    /// </summary>
    class ApplicationSettings
    {
        public string SceneDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scenes");

    }
}
