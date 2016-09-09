using System.Web.Mvc;

namespace YXERP.Areas.StyleCenter
{
    public class StyleCenterAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "StyleCenter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "StyleCenter_default",
                "StyleCenter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
