using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Models;
using WallopSceneEditor.Models.EventData;
using WallopSceneEditor.Models.EventData.Mutator;

namespace WallopSceneEditor.Services
{
    public static class NotificationHelper
    {
        public static IWindowService? WindowService;

        public static void HookMutator(ISceneMutator mutator)
        {
            mutator.OnLayoutRenamed += Mutator_OnLayoutRenamed;
            mutator.OnDirectorRenamed += Mutator_OnDirectorRenamed;
            mutator.OnActorRenamed += Mutator_OnActorRenamed;
        }

        private static void Mutator_OnLayoutRenamed(object? sender, MutatorValueChangedEventArgs<string> e)
        {
            if(e.Messages.Count > 0)
            {
                Notify(e.Messages[0]);
            }
        }

        private static void Mutator_OnDirectorRenamed(object? sender, MutatorValueChangedEventArgs<string> e)
        {
            if (e.Messages.Count > 0)
            {
                Notify(e.Messages[0]);
            }
        }

        private static void Mutator_OnActorRenamed(object? sender, MutatorValueChangedEventArgs<string> e)
        {
            if (e.Messages.Count > 0)
            {
                Notify(e.Messages[0]);
            }
        }

        public static void Notify(Message message, Action? onClick = null, Action? onClose = null)
        {
            var title = message.Title;
            if(message.Context != null)
            {
                title = $"{message.Context}: {message.Title}";
            }

            WindowService!.ShowNotification(title, message.Text, (NotificationTypes)message.MessageType, onClick, onClose);
        }
    }
}
