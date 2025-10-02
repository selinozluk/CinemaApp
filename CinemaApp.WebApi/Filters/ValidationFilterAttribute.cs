using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CinemaApp.WebApi.Filters;

public class ValidationFilterAttribute : ActionFilterAttribute // ActionFilter tabanlı model doğrulama filtresi
{
    // action filter – model validation
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid) 
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Title = "Model doğrulama hatası",
                Status = StatusCodes.Status400BadRequest
            };
            context.Result = new BadRequestObjectResult(problem);
        }
    }
}
