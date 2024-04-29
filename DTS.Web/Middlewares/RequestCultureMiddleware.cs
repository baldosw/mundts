using System.Globalization;
using System.Security.Claims;
using DTS.Common;
using DTS.DataAccess;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DTS.Web.Middlewares;

public class RequestCultureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

 
    private readonly ApplicationDbContext _dbContext;

    public RequestCultureMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        // Create a scope to resolve ApplicationDbContext
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            
            if (context.User.Identity.IsAuthenticated)
            {
       
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var employee = await dbContext.Employees.Where(u => u.UserId == userId).FirstOrDefaultAsync();
                
                var documentIncomingCount = await dbContext.Documents.Where(d => d.ModifiedBy != employee.Id && d.StatusId == (int)StatusEnum.Forwarded && d.RouteDepartmentId == employee.DepartmentId).CountAsync();
                var documentReceivedCount = await dbContext.Documents.Where(d => d.ModifiedBy == employee.Id && d.StatusId == (int)StatusEnum.Received).CountAsync();
                var documentCompletedCount = await dbContext.Documents.Where(d => d.CreatedBy == employee.Id && d.StatusId == (int)StatusEnum.Completed).CountAsync();
                var documentForwardedCount = await dbContext.Documents.Where(d => d.ModifiedBy == employee.Id && d.StatusId == (int)StatusEnum.Forwarded).CountAsync();
  
                context.Items["Received"] = documentReceivedCount;
                context.Items["Completed"] = documentCompletedCount;
                context.Items["Forwarded"] = documentForwardedCount;
                context.Items["Incoming"] = documentIncomingCount;
            }
             
            await _next(context);
 
        }
        
       
    }
 
}
