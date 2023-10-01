using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopEdit.Models;

namespace WallopEdit.Services
{
    internal class LocalEngineProcessService : IEngineProcessService
    {
        public async IAsyncEnumerable<EngineSelection> GetEngineProcessesAsync()
        {
            var procs = Process.GetProcesses();
            foreach (var process in procs)
            {
                if(process.ProcessName == "Wallop")
                {
                    yield return new EngineSelection(process.Id, "instance: " + process.Id);
                }
            }
        }
    }
}
