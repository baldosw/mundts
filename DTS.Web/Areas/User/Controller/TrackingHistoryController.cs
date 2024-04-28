using DTS.Common;
using DTS.DataAccess;
using DTS.Web.Areas.User.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTS.Web.Controllers;
[Area("User")]
public class TrackingHistoryController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public TrackingHistoryController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IActionResult> Index()
    {
        var documents = await (from document in _dbContext.TrackingHistories
            join department in _dbContext.Departments.AsNoTracking()
                on document.DepartmentId equals department.Id
            join requestType in _dbContext.RequestTypes.AsNoTracking()
                on document.RequestTypeId equals requestType.Id
                join status in _dbContext.Statuses on document.StatusId equals status.Id 
            join employeeFromDb in _dbContext.Employees on document.CreatedBy equals employeeFromDb.Id
            join routeDepartment in _dbContext.Departments on document.RouteDepartmentId equals routeDepartment.Id
            join holderEmployee in _dbContext.Employees on document.ModifiedBy equals holderEmployee.Id
                where document.TrackingCode == "NIKUPF243713"
            orderby document.Id descending 
            select new DocumentVm
            {
                Id = document.Id,
                Title = document.Title,
                Content = document.Content,
                TrackingCode = document.TrackingCode,
                Remarks = document.Remarks,
                RequestType = requestType.Title,
                Department = department.Name,
                DocumentId = document.Id,
                RequestTypeId = document.DepartmentId,
                DepartmentId = document.DepartmentId,
                StatusId = document.StatusId.Value,
                CreatedBy = document.CreatedBy,
                ModifiedBy = document.ModifiedBy,
                CurrentStatus = status.Title,
                CreatedDateString = document.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                ModifiedDateString = document.ModifiedDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                RouteDepartment = routeDepartment.Name,
                Holder = $"{holderEmployee.FirstName} {holderEmployee.MiddleName[0]} {holderEmployee.LastName} - ({routeDepartment.Name})",
                OriginalAuthor = $"{employeeFromDb.FirstName} {employeeFromDb.MiddleName[0]} {employeeFromDb.LastName} - ({department.Name})",
                CreatedTimestamp = (long)(document.CreatedDate - new DateTime(1970, 1, 1)).TotalSeconds
            }).ToListAsync();
        
        return View(documents);
    }
}