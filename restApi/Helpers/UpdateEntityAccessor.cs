using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API.Helpers
{
    public class UpdateEntityAccessor : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var data = context.HttpContext.RequestServices.GetService<ILogger<UpdateEntityAccessor>>();
            int userClaimId = int.Parse(context.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value);
            bool isAdmin = context.HttpContext.User
                .FindAll(ClaimTypes.Role).Any(c => c.Value == Role.RoleTypes.Admin.ToString());
            int sourceOwnerId = int.Parse(context.HttpContext.Request.Path.Value.Split("/")[3]);
            if (!isAdmin && userClaimId != sourceOwnerId)
            {
                context.HttpContext.Response.StatusCode = 403;
                context.HttpContext.Response.AddApplicationError("You are not resourse owner");
                context.Result = new ContentResult();
            }
            else
            {
                await next();
            }

        }
    }
}