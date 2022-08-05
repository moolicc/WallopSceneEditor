using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WallopSceneEditor.Services;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace WallopSceneEditor.ViewModels
{
    public class SceneEditViewModel : ViewModelBase
    {
        public Wallop.Shared.ECS.StoredScene? Scene { get; set; }

        public string? AttachedProcessText
        {
            get => _attachedProcessText;
            set => this.RaiseAndSetIfChanged(ref _attachedProcessText, value);
        }
        private string? _attachedProcessText = "AttachedProcess:";

        public int SelectedOutputItem
        {
            get => _selectedOutputItem;
            set => this.RaiseAndSetIfChanged(ref _selectedOutputItem, value);
        }
        private int _selectedOutputItem;

        public ObservableCollection<string> OutputList { get; set; }



        private bool _loading;

        private ISettingsService _settings;
        private IEngineService _engineService;

        public SceneEditViewModel(ISettingsService settings, IEngineService engineService)
        {
            _settings = settings;
            _engineService = engineService;
            OutputList = new ObservableCollection<string>();
        }

        protected override void OnActivate()
        {
            if(!_loading)
            {
                var appSettings = _settings.GetSettingsAsync().Result;

                _loading = true;
                if (Scene == null)
                {
                    throw new NullReferenceException();
                }

                _engineService.StartProcess(appSettings, appSettings.EngineConfig, Proc_OutputDataReceived);
                var proc = _engineService.GetEngineProcess()!;
                AttachedProcessText = $"Attached: {proc.ProcessName} ({proc.Id})";

                proc.Exited += Proc_Exited;
            }
            base.OnActivate();
        }

        private void Proc_Exited(object? sender, EventArgs e)
        {
            AttachedProcessText = $"Process exited with code {_engineService.GetEngineProcess()!.ExitCode}.";
        }

        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.Data) || string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }
            AddToOutputList(e.Data ?? "");
        }

        private void AddToOutputList(string line)
        {
            OutputList.Add(line);
            SelectedOutputItem = OutputList.Count - 1;
        }
    }
}
