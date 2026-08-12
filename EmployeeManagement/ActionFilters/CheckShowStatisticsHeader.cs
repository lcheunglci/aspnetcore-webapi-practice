using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.ActionFilters;

public class CheckShowStatisticsHeader : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers
            .ContainsKey("ShowStatistics"))
        {
            context.Result = new BadRequestResult();
            return;
        }

        if (!bool.TryParse(
                context.HttpContext.Request.Headers["ShowStatistics"].ToString(),
                out bool showStatisticsValue))
        {
            context.Result = new BadRequestResult();
            return;
        }

        if (!showStatisticsValue)
        {
            context.Result = new BadRequestResult();
        }
    }
}
