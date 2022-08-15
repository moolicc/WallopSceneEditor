using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallopSceneEditor.Plugins.SettingTypeGuiProviders;

namespace WallopSceneEditor.Plugins
{
    public static class SettingGuiProviderExtensions
    {
        public const string SETTING_READONLY = "readonly";


        public static T[] GetValues<T>(IEnumerable<KeyValuePair<string, string>>? settingArgs, string name, Func<string, T>? valueConverter = null)
        {
            if(settingArgs == null)
            {
                return Array.Empty<T>();
            }
            if(valueConverter == null)
            {
                valueConverter = s =>
                {
                    return (T)Convert.ChangeType(s, typeof(T));
                };
            }

            var results = new List<T>();

            foreach (var item in settingArgs)
            {
                if (item.Key == name)
                {
                    results.Add(valueConverter(item.Value));
                }
            }

            return results.ToArray();
        }

        public static T? GetValue<T>(IEnumerable<KeyValuePair<string, string>>? settingArgs, string name, Func<string, T?>? valueConverter = null)
        {
            if (settingArgs == null)
            {
                return default;
            }
            if (valueConverter == null)
            {
                valueConverter = s =>
                {
                    if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
                    {
                        return (T)(object)byte.Parse(s);
                    }
                    else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
                    {
                        return (T)(object)short.Parse(s);
                    }
                    else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                    {
                        return (T)(object)int.Parse(s);
                    }
                    else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                    {
                        return (T)(object)long.Parse(s);
                    }
                    else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
                    {
                        return (T)(object)float.Parse(s);
                    }
                    else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
                    {
                        return (T)(object)double.Parse(s);
                    }
                    else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                    {
                        return (T)(object)decimal.Parse(s);
                    }
                    else if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
                    {
                        return (T)(object)bool.Parse(s);
                    }
                    else if(typeof(T) == typeof(string))
                    {
                        return (T)(object)s;
                    }

                    return (T)Convert.ChangeType(s, typeof(T));
                };
            }

            foreach (var item in settingArgs)
            {
                if (item.Key == name)
                {
                    return valueConverter(item.Value);
                }
            }

            return default;
        }


        public static Tvm GetLinkedViewModel<Tvm>(ISettingValue actualValue, string vmValueMember)
            where Tvm : ViewModels.ViewModelBase, new()
        {
            if (actualValue.SettingCache is not Tvm)
            {
                var newValue = new Tvm();
                actualValue.SettingCache = newValue;

                actualValue.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(ISettingValue.Value))
                    {
                        var val = sender as ISettingValue;
                        if (val == null)
                        {
                            return;
                        }

                        var vm = val.SettingCache as Tvm;
                        if (vm == null)
                        {
                            return;
                        }
                        vm.RaisePropertyChanged(vmValueMember);
                    }
                };
            }
            var value = (Tvm)actualValue.SettingCache;
            return value;
        }
    }
}
