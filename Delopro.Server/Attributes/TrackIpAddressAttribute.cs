using Delopro.Bll.Interfaces;
using Delopro.Data.Entities;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Delopro.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TrackIpAddressAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var ip = httpContext.Connection.RemoteIpAddress?.ToString();
            var repo = httpContext.RequestServices.GetRequiredService<IRepository<Visit>>();
            var visit = new Visit
            {
                IpAddress = ip,
                Url = httpContext.Request.Path,
                VisitDate = DateTime.Now
            }; 
            
            await repo.CreateAsync(visit); 
            
            await next(); 
        } 
    }
}
