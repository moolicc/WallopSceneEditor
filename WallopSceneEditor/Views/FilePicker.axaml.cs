using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody;
using ReactiveUI.Fody.Helpers;

namespace WallopSceneEditor.Views
{
    public partial class FilePicker : UserControl
    {
        public static readonly StyledProperty<string?> WatermarkProperty =
            AvaloniaProperty.Register<FilePicker, string?>(nameof(Watermark));

        public static readonly StyledProperty<string?> FilterProperty =
            AvaloniaProperty.Register<FilePicker, string?>(nameof(Filter));

        public static readonly StyledProperty<string?> SelectedFileProperty =
            AvaloniaProperty.Register<FilePicker, string?>(nameof(SelectedFile));


        public static readonly DirectProperty<FilePicker, ICommand?> FileSelectedCommandProperty =
            AvaloniaProperty.RegisterDirect<FilePicker, ICommand?>(
                nameof(FileSelectedCommand),
                fp => fp.FileSelectedCommand,
                (fp, v) => fp.FileSelectedCommand = v);



        public string? Watermark { get; set; }


        /// <summary>
        /// The filter for the acceptable filetypes.
        /// Format example: Text files;txt,ini|All files;*
        /// </summary>
        public string? Filter { get; set; }

        public string? SelectedFile
        {
            get => GetValue(SelectedFileProperty);
            set
            {
                this.SetValue(SelectedFileProperty, value);
                FileSelectedCommand?.Execute(value);
            }
        }

        public ICommand? FileSelectedCommand { get; set; }


        public FilePicker()
        {
            InitializeComponent();
            Watermark = "File";
            Filter = null;
            SelectedFile = null;
        }

        private void InitializeComponent()
        {
            var box = new TextBox();
            AvaloniaXamlLoader.Load(this);
        }

        public void BrowseClicked(object sender, RoutedEventArgs e)
        {
            var dialog = BuildBrowse();
            var result = dialog.ShowAsync(VisualRoot as Window ?? throw new InvalidOperationException("Invalid owner!")).Result;
            if (result != null)
            {
                if (result[0] != SelectedFile)
                {
                    SelectedFile = result[0];
                }
            }
        }

        private OpenFileDialog BuildBrowse()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Open";
            dialog.AllowMultiple = false;

            if (!string.IsNullOrEmpty(SelectedFile) && File.Exists(SelectedFile))
            {
                dialog.Directory = new FileInfo(SelectedFile)?.Directory?.FullName;
                dialog.InitialFileName = Path.GetFileName(SelectedFile);
            }

            if (Filter != null)
            {
                var filters = new[] { Filter };
                if (Filter.Contains('|'))
                {
                    filters = Filter.Split(new[] { '|' }, System.StringSplitOptions.TrimEntries | System.StringSplitOptions.RemoveEmptyEntries);
                }

                foreach (var item in filters)
                {
                    var working = item;
                    var name = "";
                    var extensions = new string[1];
                    bool nameSpecified = false;

                    if (working.Contains(';'))
                    {
                        nameSpecified = true;
                        var split = item.Split(';');
                        name = split[0];
                        working = split[1];
                    }

                    if (working.Contains(','))
                    {
                        extensions = working.Split(',');
                    }
                    else
                    {
                        extensions[0] = working;
                    }

                    if (!nameSpecified)
                    {
                        for (int i = 0; i < extensions.Length; i++)
                        {
                            string? extension = extensions[i];
                            if (i > 0)
                            {
                                name += ", ";
                            }
                            name += extension;
                        }
                        name += " files";
                    }

                    var filterItem = new FileDialogFilter();

                    filterItem.Name = name;
                    filterItem.Extensions = new List<string>(extensions);

                    dialog.Filters.Add(filterItem);
                }
            }

            return dialog;
        }
    }
}
