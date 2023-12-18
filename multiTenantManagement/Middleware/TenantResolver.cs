using Microsoft.AspNetCore.Mvc.Controllers;
using multiTenantManagement.Models.Enums;
using multiTenantManagement.Models.Tenants;
using multiTenantManagement.Services.Contract;

namespace multiTenantManagement.Middleware
{
    public class TenantResolver
    {
        //    private readonly ICurrentTenantService _tenantService;
        //    public TenantResolver(ICurrentTenantService tenantService)
        //    {
        //        _tenantService = tenantService;
        //    }


        //    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        //    {
        //        var tenantId = context.User.Claims.Where(x => x.Type.ToLower() == "tenantId").SingleOrDefault().Value;
        //        if (!string.IsNullOrEmpty(tenantId))
        //        {
        //            await _tenantService.SetTenant(tenantId);
        //        }
        //        await next(context);
        //    }
        //}
        private readonly RequestDelegate _next;
        public TenantResolver(RequestDelegate next)
        {
            _next = next;
        }

        // Get Tenant Id from incoming requests 
        public async Task InvokeAsync(HttpContext context, ICurrentTenantService currentTenantService)
        {
            var endpoint = context.Request.HttpContext.GetEndpoint();
            var currentEndpoint = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>()?.ActionName;

            var x = context.Request.HttpContext.User.Identity.IsAuthenticated;
            //if(currentEndpoint == "Login" || currentEndpoint == "Register")
            //{
            //    await _next(context);

            //}
            if (!context.Request.HttpContext.User.Identity.IsAuthenticated)
            {
                await _next(context);
            }
            
            else
            {
                if (context.User.Claims == null)
                {
                    return;
                }
                var tenant = context.User.Claims.Where(x => x.Type.ToLower() == "tenantid").FirstOrDefault();

                var tenantId = tenant.Value;
                if (!string.IsNullOrEmpty(tenantId))
                {
                    await currentTenantService.SetTenant(tenantId);
                    await _next(context);
                }
            }
        }


    }
    public static class TenantMiddlewareExtension
    {
        public static IApplicationBuilder UseTenantResolver(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TenantResolver>();
        }
    }
}
