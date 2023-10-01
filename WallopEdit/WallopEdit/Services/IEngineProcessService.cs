using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.Services
{
    public interface IEngineProcessService
    {
        IAsyncEnumerable<Models.EngineSelection> GetEngineProcessesAsync();
    }
}
