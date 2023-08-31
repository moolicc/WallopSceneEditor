using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.Models
{
    public record struct ListedScene(int ListedId, string SceneSource, string SceneName);
}
