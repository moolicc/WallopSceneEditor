using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Models.EventData
{
    public class ValueChangedEventArgs<TValue> : EventArgs
    {
        public TValue NewValue { get; private set; }
        public TValue OldValue { get; private set; }

        public ValueChangedEventArgs(TValue oldValue, TValue newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class ValueChangedAttemptEventArgs<TValue> : ValueChangedEventArgs<TValue>
    {
        public bool Success { get; init; }

        public ValueChangedAttemptEventArgs(TValue oldValue, TValue newValue, bool success)
            : base(oldValue, newValue)
        {
            Success = success;
        }
    }
}
