using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Models;

namespace WallopSceneEditor.Services
{
    public static class OutputHelper
    {
        public static event Action<Message>? OnLog;


        public static void LogWarning(string text, string title = "")
        {
            Log(new Message("", title, text, MessageType.Warning));
        }

        public static void LogError(string text, string title = "")
        {
            Log(new Message("", title, text, MessageType.Error));
        }




        public static void LogWarning<T>(string text, string title = "")
        {
            var name = typeof(T).Name.Replace("ViewModel", "").Replace("view", "");
            Log(new Message(name, title, text, MessageType.Warning));
        }
        public static void LogError<T>(string text, string title = "")
        {
            var name = typeof(T).Name.Replace("ViewModel", "").Replace("view", "");
            Log(new Message(name, title, text, MessageType.Error));
        }




        public static void Log(string text, string title = "", string context = "", MessageType type = MessageType.Information)
        {
            Log(new Message(context, title, text, type));
        }
        public static void Log<T>(string text, string title = "", MessageType type = MessageType.Information)
        {
            var name = typeof(T).Name.Replace("ViewModel", "").Replace("view", "");
            Log(new Message(name, title, text, type));
        }




        public static void Log(Message message)
        {
            OnLog?.Invoke(message);
        }
    }
}
