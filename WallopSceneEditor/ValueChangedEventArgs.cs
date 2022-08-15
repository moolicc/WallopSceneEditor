using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor
{
    public class ValueChangedEventArgs<TValue> : EventArgs
    {
        public TValue OldValue { get; private set; }

        public ValueChangedEventArgs(TValue oldValue)
        {
            OldValue = oldValue;
        }
    }
}
