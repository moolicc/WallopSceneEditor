using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopSceneEditor.Services
{
    public class HardCodedPluginService : IPluginService
    {
        public IEnumerable<T> GetImplementations<T>()
        {
            if(typeof(T) == typeof(Plugins.ISettingTypeGuiProvider))
            {
                yield return (T)(object)new Plugins.SettingTypeGuiProviders.ChoiceGuiProvider();
                yield return (T)(object)new Plugins.SettingTypeGuiProviders.FileGuiProvider();
                yield return (T)(object)new Plugins.SettingTypeGuiProviders.RealNumberGuiProvider();
                yield return (T)(object)new Plugins.SettingTypeGuiProviders.StringGuiProvider();
                yield return (T)(object)new Plugins.SettingTypeGuiProviders.BoolGuiProvider();
            }
        }
    }
}
