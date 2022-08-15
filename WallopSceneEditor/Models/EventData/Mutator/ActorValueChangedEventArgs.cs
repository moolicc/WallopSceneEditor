using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models.EventData.Mutator
{
    public class ActorValueChangedEventArgs<TValue> : MutatorValueChangedEventArgs<TValue>
    {
        public string ParentLayout { get; init; }

        public ActorValueChangedEventArgs(TValue oldValue, TValue newValue, bool failed, string parentLayout)
            : base(oldValue, newValue, false)
        {
            ParentLayout = parentLayout;
        }
    }
}
