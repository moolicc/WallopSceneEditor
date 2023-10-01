using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallopEdit.Presentation
{
    public class LandingPageNavMenuTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SceneItemTemplate { get; set; }
        public DataTemplate ControlItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if(item is NavigationViewItem navItem && navItem.DataContext is NavItemControlButtonModel)
            {
                return ControlItemTemplate;
            }
            return SceneItemTemplate;
        }
    }
}
