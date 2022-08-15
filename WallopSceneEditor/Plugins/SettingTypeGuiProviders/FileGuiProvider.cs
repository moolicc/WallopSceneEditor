using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Services;
using ReactiveUI;

namespace WallopSceneEditor.Plugins.SettingTypeGuiProviders
{
    internal class FileGuiProvider : ISettingTypeGuiProvider
    {
        public string TypeName => "file";

        public bool HandlesPopoutDialog => true;

        public Control GetInlineDisplayControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var vm = new FileViewModel(value);
            vm.SelectedFile = value.Value;

            value.PropertyChanged += (s, e) =>
            {
                vm.SelectedFile = value.Value;
            };

            var control = new FileControl();
            control.DataContext = vm;
            return control;
        }

        public Control? GetInlineEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            return null;
        }

        public Control? GetPopoutEditControl(ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            return null;
        }

        public async Task<bool> OnShowPopoutDialogAsync(IWindowService windowService, ISettingValue value, IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            var filters = ParseFilter(settingArgs);
            var file = await windowService.ShowFileDialogAsync<OpenFileDialog>(d =>
            {
                d.AllowMultiple = false;
                d.Title = "Select a File";
                d.Filters = filters?.Select(f => new FileDialogFilter() { Name = f.Label, Extensions = new List<string>(f.Extensions) }).ToList();
            }).ConfigureAwait(false);

            value.Value = file;
            return true;
        }

        private (string Label, string[] Extensions)[]? ParseFilter(IEnumerable<KeyValuePair<string, string>>? settingArgs)
        {
            if(settingArgs == null)
            {
                return null;
            }

            var results = new List<(string Label, string[] Extensions)>();
            var filterSettings = settingArgs.Where(s => s.Key == "filter");
            foreach (var item in filterSettings)
            {
                var value = item.Value;
                var extensionsFlat = value;

                var label = "";
                var extensions = new string[1] { extensionsFlat };
                if(value.Contains(';'))
                {
                    var split = value.Split(';', 2);
                    label = split[0];
                    extensionsFlat = split[1];
                }

                if(extensionsFlat.Contains(','))
                {
                    extensions = extensionsFlat.Split(',');
                }

                if(string.IsNullOrEmpty(label))
                {
                    label = string.Join(", ", extensionsFlat) + " files";
                }

                results.Add((label, extensions));
            }

            return results.ToArray();
        }


        
    }
}
