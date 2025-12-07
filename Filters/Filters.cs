using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace software_engineering.Filters
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                context.Result = new RedirectToActionResult("Login", "Users", null);
                //returns to login page if not admin
            }
        }
    }

    public class ClinicianOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("UserRole");
            if (role != "clincian") // match the enum string value
            {
                context.Result = new RedirectToActionResult("Login", "Users", null);
            }
        }
    }
}
