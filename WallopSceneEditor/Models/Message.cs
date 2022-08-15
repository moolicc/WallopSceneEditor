using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models
{
    public enum MessageType
    {
        Information = 0,
        Success = 1,
        Warning = 2,
        Error = 3
    }

    public readonly record struct Message(string Context, string Title, string Text, MessageType MessageType);
}
