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
public class DocumentStatusController : Controller
{

    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DocumentStatusController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
    {
        
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Outgoing()
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();
        ViewData["employeeId"] = employee.Id;
        return View();
    }
    
    public async Task<IActionResult> Incoming()
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();
        ViewData["employeeId"] = employee.Id;
        return View();
    }
    
    #region API CALLS 
    
    public async Task<IActionResult> GetOutgoingDocuments()
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();

        var documents = await (from document in _dbContext.Documents
                join department in _dbContext.Departments.AsNoTracking()
                    on document.DepartmentId equals department.Id
                join requestType in _dbContext.RequestTypes.AsNoTracking()
                    on document.RequestTypeId equals requestType.Id
            where document.CreatedBy == employee.Id && document.StatusId == (int)StatusEnum.Forwarded
                                                    && document.StatusId != (int)StatusEnum.Received
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
                CreatedTimestamp = (long)(document.CreatedDate - new DateTime(1970, 1, 1)).TotalSeconds
            }
              ).ToListAsync();
       
        var json = new { data = documents, success = true };
        return Json(json);
    }
    
    public async Task<IActionResult> GetIncomingDocuments()
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();

        var documents = await (from document in _dbContext.Documents
                join department in _dbContext.Departments.AsNoTracking()
                    on document.DepartmentId equals department.Id
                join requestType in _dbContext.RequestTypes.AsNoTracking()
                    on document.RequestTypeId equals requestType.Id
                where document.CreatedBy != employee.Id  && document.StatusId != (int)StatusEnum.Received
                                                         && employee.DepartmentId == document.RouteDepartmentId
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
                    CreatedTimestamp = (long)(document.CreatedDate - new DateTime(1970, 1, 1)).TotalSeconds
                }
            ).ToListAsync();
       
        var json = new { data = documents, success = true };
        return Json(json);
    }
    
     
    [HttpPut]
    public async Task<IActionResult> UpdateDocumentToReceive([FromBody] DocumentStatusVm documentStatus)
    {
        var document = await _dbContext.Documents.Where(d => d.Id == documentStatus.DocumentId).FirstOrDefaultAsync();
        document.ModifiedBy = documentStatus.EmployeeId;
        document.ModifiedDate = DateTime.Now;
        document.StatusId = (int)StatusEnum.Received;

        _dbContext.Update(document);
        await _dbContext.SaveChangesAsync();
        
        var json = new {  success = true };
        return Json(json);
    }
    
    #endregion
}

 