using System.Web.Mvc;

namespace YXERP.Areas.IntFactoryModel
{
    public class IntFactoryModelAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "IntFactoryModel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "IntFactoryModel_default",
                "IntFactoryModel/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
