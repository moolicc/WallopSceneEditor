using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models.EventData.Mutator
{
    public abstract class MutatorEventArgs : EventArgs, IMessageContainer
    {
        public List<Message> Messages { get; set; }
        public bool HasError { get; set; }

        protected MutatorEventArgs()
        {
            Messages = new List<Message>();
            HasError = false;
        }
    }
}
