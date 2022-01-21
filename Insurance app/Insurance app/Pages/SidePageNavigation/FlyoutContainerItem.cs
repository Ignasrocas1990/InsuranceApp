using System;

namespace Insurance_app.Pages.SidePageNavigation
{
    public class FlyoutContainerItem
    {
        public FlyoutContainerItem()
        {
            TargetType = typeof(FlyoutContainerItem);
        }
        public string IconSource { get; set; }

        public string Title { get; set; }
        public Type TargetType { get; set; }
    }
}