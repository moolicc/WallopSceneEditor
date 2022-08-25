using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Models;
using ReactiveUI;

namespace WallopSceneEditor.ViewModels
{
    public class OutputLineViewModel : ViewModelBase
    {
        public static string ICON_INFORMATION = "ℹ️";
        public static string ICON_WARNING = "❕";
        public static string ICON_ERROR = "⚠️";
        public static string ICON_SUCCESS = "✔️";


        public Message? Message
        {
            get => _message;
            set
            {
                if(_message == value)
                {
                    return;
                }

                _message = value;
                if(_message != null)
                {
                    Context = _message.Value.Context;
                    Title = _message.Value.Title;
                    Text = _message.Value.Text;

                    switch (_message.Value.MessageType)
                    {
                        case MessageType.Success:
                            Icon = ICON_SUCCESS;
                            Foreground = Colors.DarkGreen;
                            break;
                        case MessageType.Warning:
                            Icon = ICON_WARNING;
                            Foreground = Colors.Goldenrod;
                            break;
                        case MessageType.Error:
                            Icon = ICON_ERROR;
                            Foreground = Colors.Red;
                            break;
                        case MessageType.Information:
                        default:
                            Icon = ICON_INFORMATION;
                            Foreground = Colors.Black;
                            break;
                    }
                    Icon += "  ";
                }
                else
                {
                    Context = null;
                    Title = null;
                    Text = null;
                    Icon = null;
                    Foreground = Colors.Black;
                }

                this.RaisePropertyChanged(nameof(Context));
                this.RaisePropertyChanged(nameof(Title));
                this.RaisePropertyChanged(nameof(Text));
                this.RaisePropertyChanged(nameof(Icon));
                this.RaisePropertyChanged(nameof(Foreground));
                this.RaisePropertyChanged(nameof(Header));
            }
        }

        public string? Header
        {
            get
            {
                string? result = null;
                if(!string.IsNullOrEmpty(Context))
                {
                    result += "[" + Context;
                }
                if(!string.IsNullOrEmpty(Title))
                {
                    if(!string.IsNullOrEmpty(Context))
                    {
                        result += " : " + Title;
                    }
                    else
                    {
                        result += "[" + Title;
                    }
                    result += "]  ";
                }
                else if(!string.IsNullOrEmpty(Context))
                {
                    result += "]  ";
                }
                return result;
            }
        }


        public string? Context { get; private set; }

        public string? Title { get; private set; }

        public string? Text { get; private set; }

        public string? Icon { get; private set; }

        public Color Foreground { get; private set; }

        private Message? _message;

        public OutputLineViewModel()
        {
        }
    }
}
