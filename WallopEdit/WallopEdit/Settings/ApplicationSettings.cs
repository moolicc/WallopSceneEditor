using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.Settings
{
    /// <summary>
    /// Represents settings related to WallopEdit as an isolated application.
    /// </summary>
    public class ApplicationSettings
    {

        public string SceneDirectory
        {
            get => _sceneDirectory;
            set => SetNewDirectory(value);
        }
        private string _sceneDirectory = "";

        public ApplicationSettings()
        {
            SetNewDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scenes"));
        }

        private void SetNewDirectory(string value)
        {
            if(!Directory.Exists(value))
            {
                Directory.CreateDirectory(value);
            }
            _sceneDirectory = value;
        }

    }
}
