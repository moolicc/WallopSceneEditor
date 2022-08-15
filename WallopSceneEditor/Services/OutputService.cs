using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    public static class OutputService
    {
        public static event Action<string>? OnLog;

        public static void Log(string text)
        {
            OnLog?.Invoke(text);
        }
    }
}
