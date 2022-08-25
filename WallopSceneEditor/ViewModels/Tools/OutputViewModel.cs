using Dock.Model.ReactiveUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Models;
using ReactiveUI;

namespace WallopSceneEditor.ViewModels.Tools
{
    public class OutputViewModel : Tool
    {
        public ObservableCollection<OutputLineViewModel> Messages { get; set; }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
        }

        private int _selectedIndex;

        public OutputViewModel()
        {
            Messages = new ObservableCollection<OutputLineViewModel>();
            Services.OutputHelper.OnLog += OutputHelper_OnLog;
        }

        private void OutputHelper_OnLog(Message message)
        {
            var atBottom = SelectedIndex == Messages.Count - 1;

            Messages.Add(new OutputLineViewModel() { Message = message });
            if (Messages.Count == 1)
            {
                SelectedIndex = 0;
            }

            if(atBottom)
            {
                SelectedIndex = Messages.Count - 1;
            }
        }

        private string GetMessageString(OutputLineViewModel line)
        {
            var result = "";
            var message = line.Message!.Value;

            switch (message.MessageType)
            {
                case MessageType.Information:
                    result = "[Information] ";
                    break;
                case MessageType.Success:
                    result = "[Success] ";
                    break;
                case MessageType.Warning:
                    result = "[Warning] ";
                    break;
                case MessageType.Error:
                    result = "[Error] ";
                    break;
                default:
                    break;
            }

            result += line.Header;
            result += message.Text;

            return result;
        }
    }
}
