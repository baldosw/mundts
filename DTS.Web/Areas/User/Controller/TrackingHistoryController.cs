using System.Security.Claims;
using DTS.Common;
using DTS.DataAccess;
using DTS.Web.Areas.User.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTS.Web.Controllers;
[Area("User")]
[Authorize]
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
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee =  await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();

        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
        ViewData["employeeId"] = employee.Id;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Index(string search)
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee =  await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();

        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
        ViewData["employeeId"] = employee.Id;
        
        var documents = await (from document in _dbContext.TrackingHistories
            join department in _dbContext.Departments.AsNoTracking()
                on document.DepartmentId equals department.Id
            join requestType in _dbContext.RequestTypes.AsNoTracking()
                on document.RequestTypeId equals requestType.Id
                join status in _dbContext.Statuses on document.StatusId equals status.Id 
            join employeeFromDb in _dbContext.Employees on document.CreatedBy equals employeeFromDb.Id
            join routeDepartment in _dbContext.Departments on document.RouteDepartmentId equals routeDepartment.Id
            join holderEmployee in _dbContext.Employees on document.ModifiedBy equals holderEmployee.Id
                where document.TrackingCode == search
            orderby document.ModifiedDate descending 
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