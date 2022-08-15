using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models
{
    public interface IMessageContainer
    {
        List<Message> Messages { get; set; }
        bool HasError { get; set; }
    }
}
