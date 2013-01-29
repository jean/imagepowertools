using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Amba.ImagePowerTools
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .Add(T("Settings"), menu => menu
                    .Add(T("Image Power Tools"), "1.0", x => x
                        .Action("Settings", "Admin", new { area = "Amba.ImagePowerTools" })
                        .Permission(StandardPermissions.SiteOwner)
                    ));
        }
    }
}