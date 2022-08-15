using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models.EventData.Mutator
{
    public class MutatorValueChangedEventArgs<TValue> : MutatorEventArgs
    {
        public TValue OldValue { get; set; }
        public TValue NewValue { get; set; }

        public MutatorValueChangedEventArgs(TValue oldValue, TValue newValue, bool failed = false)
        {
            OldValue = oldValue;
            NewValue = newValue;
            HasError = failed;
        }
    }
}
