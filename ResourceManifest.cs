using Orchard.UI.Resources;

namespace Amba.ImagePowerTools
{
    public class ResourceManifest : IResourceManifestProvider
    {
        /*
         * Script.Require("jQueryUI_Sortable").AtHead();
    Script.Include("angular-last.min.js").AtHead();
    Script.Include("angular-ui.min.js").AtHead();
    Script.Include("AmbaSortableList.js").AtHead();
    Script.Include("MultipickerDashboard.js").AtHead();
    Script.Include("ExtraEvents.js").AtHead();
    Style.Include("dashboard.css").AtHead();
         */
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineStyle("IPT_DashBoard").SetUrl("dashboard.css");
            manifest.DefineScript("IPT_AngularLast").SetUrl("angular-last.min.js").SetDependencies("jQuery");
            manifest.DefineScript("IPT_ExtraEvents").SetUrl("ExtraEvents.js").SetDependencies("IPT_AngularLast");
            manifest.DefineScript("IPT_AngularUI").SetUrl("angular-ui.min.js").SetDependencies("IPT_AngularLast");
            manifest.DefineScript("IPT_AmbaSortableList").SetUrl("AmbaSortableList.js").SetDependencies("IPT_AngularUI", "jQueryUI_Sortable", "IPT_ExtraEvents");
            manifest.DefineScript("IPT_MultipickerDashboard").SetUrl("MultipickerDashboard.js").SetDependencies("IPT_AmbaSortableList", "IPT_AngularUI");
            
        }
    }
}
