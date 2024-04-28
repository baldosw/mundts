using System.Security.Claims;
using DTS.Common;
using DTS.DataAccess;
using DTS.Models;
using DTS.Web.Areas.User.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
   
        var employee = await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();
        ViewData["employeeId"] = employee.Id;
        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
        
        return View();
    }
    
    public async Task<IActionResult> Incoming()
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();
        ViewData["employeeId"] = employee.Id;
        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
        return View();
    }
    
    public async Task<IActionResult> Received()
    {
        DocumentVm documentVm = new DocumentVm();
        
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();
        ViewData["employeeId"] = employee.Id;
        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
        
        documentVm.Departments = _dbContext.Departments.Select(entity => new SelectListItem
        {
            Text = entity.Name,
            Value = entity.Id.ToString()
        }).ToList();
        
        return View(documentVm);
    }
    
    public async Task<IActionResult> Completed()
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();
        ViewData["employeeId"] = employee.Id;
        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
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
                    join status in _dbContext.Statuses on document.StatusId equals status.Id
                join employeeFromDb in _dbContext.Employees on document.CreatedBy equals employeeFromDb.Id
                join routeDepartment in _dbContext.Departments on document.RouteDepartmentId equals routeDepartment.Id
                where  
                       document.StatusId == (int)StatusEnum.Forwarded 
                      && document.StatusId != (int)StatusEnum.Received 
                      && document.ModifiedBy == employee.Id  
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
                    CurrentStatus = status.Title,
                    CreatedDateString = document.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    RouteDepartment = routeDepartment.Name,
                    OriginalAuthor = $"{employeeFromDb.FirstName} {employeeFromDb.MiddleName[0]} {employeeFromDb.LastName} - ({department.Name})", 
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
                    join status in _dbContext.Statuses on document.StatusId equals status.Id
                join employeeFromDb in _dbContext.Employees on document.CreatedBy equals employeeFromDb.Id
                where document.CreatedBy != employee.Id  
                      && document.StatusId != (int)StatusEnum.Received
                      && document.StatusId != (int)StatusEnum.Cancelled
                      && document.StatusId != (int)StatusEnum.Completed
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
                    CurrentStatus = status.Title,
                    CreatedDateString = document.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    OriginalAuthor = $"{employeeFromDb.FirstName} {employeeFromDb.MiddleName[0]} {employeeFromDb.LastName} - ({department.Name})",
                    CreatedTimestamp = (long)(document.CreatedDate - new DateTime(1970, 1, 1)).TotalSeconds
                }
            ).ToListAsync();
       
        var json = new { data = documents, success = true };
        return Json(json);
    }
    
    public async Task<IActionResult> GetReceivedDocuments()
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();

        var documents = await (
            from document in _dbContext.Documents
            join department in _dbContext.Departments.AsNoTracking()
                on document.DepartmentId equals department.Id
            join requestType in _dbContext.RequestTypes.AsNoTracking()
                on document.RequestTypeId equals requestType.Id
            join status in _dbContext.Statuses on document.StatusId equals status.Id
            join employeeFromDb in _dbContext.Employees on document.CreatedBy equals employeeFromDb.Id
            where document.StatusId == (int)StatusEnum.Received
                  && employee.DepartmentId == document.RouteDepartmentId
                  && (document.ModifiedBy == employee.Id)
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
                CurrentStatus = status.Title,
                CreatedDateString = document.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                OriginalAuthor = $"{employeeFromDb.FirstName} {employeeFromDb.MiddleName[0]} {employeeFromDb.LastName} - ({department.Name})",
                CreatedTimestamp = (long)(document.CreatedDate - new DateTime(1970, 1, 1)).TotalSeconds
            }
        ).ToListAsync();
       
        var json = new { data = documents, success = true };
        return Json(json);
    }
    
    [HttpGet]
    public async Task<IActionResult>  GetCompletedDocuments()
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();
        
        var documents = await (from document in _dbContext.Documents
            join department in _dbContext.Departments.AsNoTracking()
                on document.DepartmentId equals department.Id
            join requestType in _dbContext.RequestTypes.AsNoTracking()
                on document.RequestTypeId equals requestType.Id
            join status in _dbContext.Statuses on document.StatusId equals status.Id
            join employeeFromDb in _dbContext.Employees on document.CreatedBy equals employeeFromDb.Id
            where document.CreatedBy == employee.Id
                  && document.StatusId == (int)StatusEnum.Completed
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
                OriginalAuthor = $"{employeeFromDb.FirstName} {employeeFromDb.MiddleName[0]} {employeeFromDb.LastName} - ({department.Name})",
                CreatedTimestamp = (long)(document.CreatedDate - new DateTime(1970, 1, 1)).TotalSeconds
            }).ToListAsync();
        
        var dataJson = new { data = documents };

        return Ok(dataJson);
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
        
        //Transaction History
        TrackingHistory transactionHistory = new TrackingHistory();
        transactionHistory.DepartmentId = document.DepartmentId;
        transactionHistory.Title = document.Title;
        transactionHistory.TrackingCode = document.TrackingCode;
        transactionHistory.Content = document.Content;
        transactionHistory.Remarks = document.Remarks;
        transactionHistory.CreatedDate = document.CreatedDate;
        transactionHistory.CreatedBy = document.CreatedBy;
        transactionHistory.RouteDepartmentId = document.RouteDepartmentId;
        transactionHistory.RequestTypeId = document.RequestTypeId;
        transactionHistory.DocumentId = document.Id;
        
        transactionHistory.ModifiedBy = documentStatus.EmployeeId;
        transactionHistory.ModifiedDate = DateTime.Now;
        transactionHistory.StatusId = (int)StatusEnum.Received;
 
        await _dbContext.TrackingHistories.AddAsync(transactionHistory);
        await _dbContext.SaveChangesAsync();
         
        var json = new {  success = true };
        return Json(json);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateDocumentToCancel([FromBody] DocumentStatusVm documentStatus)
    {
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();
        var document = await _dbContext.Documents.Where(d => d.Id == documentStatus.DocumentId).FirstOrDefaultAsync();

        if (document.StatusId == (int)StatusEnum.Completed || document.StatusId == (int)StatusEnum.Forwarded &&
            document.ModifiedBy != document.CreatedBy)
        {
            var failJsonData = new {  success = false, message = "You are not allowed to cancel a forwarded/completed document " };
            return BadRequest(failJsonData);
        }
         
        if (employee.Id == document.CreatedBy)
        {
            document.ModifiedBy = documentStatus.EmployeeId;
            document.ModifiedDate = DateTime.Now;
            document.StatusId = (int)StatusEnum.Cancelled;
            _dbContext.Update(document);
            await _dbContext.SaveChangesAsync();
            
            //Transaction History
            TrackingHistory transactionHistory = new TrackingHistory();
            transactionHistory.DepartmentId = document.DepartmentId;
            transactionHistory.Title = document.Title;
            transactionHistory.TrackingCode = document.TrackingCode;
            transactionHistory.Content = document.Content;
            transactionHistory.Remarks = document.Remarks;
            transactionHistory.CreatedDate = document.CreatedDate;
            transactionHistory.CreatedBy = document.CreatedBy;
            transactionHistory.RouteDepartmentId = document.RouteDepartmentId;
            transactionHistory.RequestTypeId = document.RequestTypeId;
            transactionHistory.DocumentId = document.Id;
        
            transactionHistory.ModifiedBy = documentStatus.EmployeeId;
            transactionHistory.ModifiedDate = DateTime.Now;
            transactionHistory.StatusId = (int)StatusEnum.Cancelled;
 
            await _dbContext.TrackingHistories.AddAsync(transactionHistory);
            await _dbContext.SaveChangesAsync();
        
            var json = new {  success = true };
            return Ok(json);
        }
        else
        {
            var failJsonData = new {  success = false, message = "You are not allowed to cancel a forwarded/completed document " };
            return BadRequest(failJsonData);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateDocumentToForward([FromBody] DocumentStatusVm documentStatus)
    {
        var document = await _dbContext.Documents.Where(d => d.Id == documentStatus.DocumentId).FirstOrDefaultAsync();
        document.ModifiedBy = documentStatus.EmployeeId;
        document.ModifiedDate = DateTime.Now;
        document.StatusId = (int)StatusEnum.Forwarded;
        document.Remarks = documentStatus.Remarks;
        document.RouteDepartmentId = documentStatus.RouteDepartmentId;

        _dbContext.Update(document);
        await _dbContext.SaveChangesAsync();
        
        //Transaction History
        TrackingHistory transactionHistory = new TrackingHistory();
        transactionHistory.DepartmentId = document.DepartmentId;
        transactionHistory.Title = document.Title;
        transactionHistory.TrackingCode = document.TrackingCode;
        transactionHistory.Content = document.Content;
        transactionHistory.Remarks = documentStatus.Remarks;
        transactionHistory.CreatedDate = document.CreatedDate;
        transactionHistory.CreatedBy = document.CreatedBy;
        transactionHistory.RouteDepartmentId = documentStatus.RouteDepartmentId;
        transactionHistory.RequestTypeId = document.RequestTypeId;
        transactionHistory.DocumentId = document.Id;
        
        transactionHistory.ModifiedBy = documentStatus.EmployeeId;
        transactionHistory.ModifiedDate = DateTime.Now;
        transactionHistory.StatusId = (int)StatusEnum.Forwarded;
 
        await _dbContext.TrackingHistories.AddAsync(transactionHistory);
        await _dbContext.SaveChangesAsync();
        
        var json = new {  success = true };
        return Json(json);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateDocumentToComplete([FromBody] DocumentStatusVm documentStatus)
    {
         
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();
        var document = await _dbContext.Documents.Where(d => d.Id == documentStatus.DocumentId).FirstOrDefaultAsync();
        
        if (employee.Id == document.CreatedBy && document.StatusId == (int)StatusEnum.Received)
        {
            document.ModifiedBy = documentStatus.EmployeeId;
            document.ModifiedDate = DateTime.Now;
            document.StatusId = (int)StatusEnum.Completed;
            document.Remarks = "COMPLETED";
            _dbContext.Update(document);
            await _dbContext.SaveChangesAsync();
            
            //Transaction History
            TrackingHistory transactionHistory = new TrackingHistory();
            transactionHistory.DepartmentId = document.DepartmentId;
            transactionHistory.Title = document.Title;
            transactionHistory.TrackingCode = document.TrackingCode;
            transactionHistory.Content = document.Content;
            transactionHistory.CreatedDate = document.CreatedDate;
            transactionHistory.CreatedBy = document.CreatedBy;
            transactionHistory.RouteDepartmentId = employee.DepartmentId;
            transactionHistory.RequestTypeId = document.RequestTypeId;
            transactionHistory.DocumentId = document.Id;
        
            transactionHistory.ModifiedBy = documentStatus.EmployeeId;
            transactionHistory.ModifiedDate = DateTime.Now;
            transactionHistory.StatusId = (int)StatusEnum.Completed;
            transactionHistory.Remarks = "COMPLETED";
            
 
            await _dbContext.TrackingHistories.AddAsync(transactionHistory);
            await _dbContext.SaveChangesAsync();
        
            var json = new {  success = true };
            return Ok(json);
        }
        else
        {
            var failJsonData = new {  success = false, message = "You are not allowed to complete this document " };
            return BadRequest(failJsonData);
        }
    }
    
    #endregion
}

 