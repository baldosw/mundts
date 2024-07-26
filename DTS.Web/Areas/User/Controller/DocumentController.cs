using System.Security.Claims;
using DTS.Common;
using DTS.Common.DataTables;
using DTS.DataAccess;
using DTS.Models;
using DTS.Web.Areas.User.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace DTS.Web.Controllers;

[Area("User")]
[Authorize]
public class DocumentController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DocumentController> _logger;
    
    public DocumentController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<DocumentController> logger)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        DocumentVm documentVm = new DocumentVm();
        documentVm.RequestTypes = _dbContext.RequestTypes.Select(entity => new SelectListItem
        {
            Text = entity.Title,
            Value = entity.Id.ToString()
        }).ToList();
        
        documentVm.Departments = _dbContext.Departments.Select(entity => new SelectListItem
        {
            Text = entity.Name,
            Value = entity.Id.ToString()
        }).ToList();
         
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee =  await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();

        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
        ViewData["employeeId"] = employee.Id;
         
        if (HttpContext.Items.ContainsKey("Received"))
        {
            var receivedMiddlewareValue = HttpContext.Items["Received"];
            var forwardedMiddlewareValue = HttpContext.Items["Forwarded"];
            var completedMiddlewareValue = HttpContext.Items["Completed"];
            var incomingMiddlewareValue = HttpContext.Items["Incoming"];
            
            ViewData["Received"] = receivedMiddlewareValue;
            ViewData["Forwarded"] = forwardedMiddlewareValue;
            ViewData["Completed"] = completedMiddlewareValue;
            ViewData["Incoming"] = incomingMiddlewareValue;
        }
 
        return View(documentVm);
    }
    
    public async Task<IActionResult> Create()
    {
        DocumentVm documentVm = new DocumentVm();
        documentVm.TrackingCode = TrackingCodeGenerator.Generate();

        documentVm.RequestTypes = _dbContext.RequestTypes.Select(rt => new SelectListItem
        {
            Value = rt.Id.ToString(),
            Text = rt.Title
        });
        
        string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var employee = await _dbContext.Employees.Include(e => e.Department).Where(e => e.UserId == userId).FirstOrDefaultAsync();

        ViewData["FirstName"] = employee.FirstName;
        ViewData["LastName"] = employee.LastName;
        ViewData["DepartmentShort"] = employee.Department.ShortName;
        return View(documentVm);
    }


    #region Documents Api

    [HttpGet]
    public async Task<IActionResult>  GetDocuments()
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
            join holderEmployee in _dbContext.Employees on document.ModifiedBy equals holderEmployee.Id
                where document.CreatedBy == employee.Id
                && document.StatusId != (int)StatusEnum.Completed
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
                CurrentStatus = $"{status.Title} - {routeDepartment.Name}",
                CreatedDateString = document.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                RouteDepartment = routeDepartment.Name,
                Holder = $"{holderEmployee.FirstName} {holderEmployee.MiddleName[0]} {holderEmployee.LastName}  ",
                OriginalAuthor = $"{employeeFromDb.FirstName} {employeeFromDb.MiddleName[0]} {employeeFromDb.LastName} - ({department.Name})",
                CreatedTimestamp = (long)(document.CreatedDate - new DateTime(1970, 1, 1)).TotalSeconds
            }).ToListAsync();
        
        var dataJson = new { data = documents };

        return Ok(dataJson);
    }
    
    public async Task<IActionResult>  GetDocument(int id)
    {
        var documentFromDb = await (from document in _dbContext.Documents
            join department in _dbContext.Departments.AsNoTracking()
                on document.DepartmentId equals department.Id
            join requestType in _dbContext.RequestTypes.AsNoTracking()
                on document.RequestTypeId equals requestType.Id
                join status in _dbContext.Statuses on document.StatusId equals status.Id
            join employeeFromDb in _dbContext.Employees on document.CreatedBy equals employeeFromDb.Id
            join routeDepartment in _dbContext.Departments on document.RouteDepartmentId equals routeDepartment.Id
            join holderEmployee in _dbContext.Employees on document.ModifiedBy equals holderEmployee.Id
            orderby document.Id descending 
            where document.Id == id
            select new DocumentVm
            {
                Id = document.Id,
                Title = document.Title,
                Content = document.Content,
                TrackingCode = document.TrackingCode,
                Remarks = document.Remarks,
                RequestType = requestType.Title,
                Department = department.Name,
                RequestTypeId = requestType.Id,
                DepartmentId = department.Id,
                DepartmentName = department.Name,
                RequestTypeTitle = requestType.Title,
                RouteDepartmentId = document.RouteDepartmentId,
                StatusId = document.StatusId.Value,
                OriginalAuthor = $"{employeeFromDb.FirstName} {employeeFromDb.MiddleName[0]} {employeeFromDb.LastName} - ({department.Name})",
                CurrentStatus = $"{status.Title} - {routeDepartment.Name}",
                RouteDepartment = routeDepartment.Name,
                Holder = $"{holderEmployee.FirstName} {holderEmployee.MiddleName[0]} {holderEmployee.LastName} ",
                CreatedDateString = document.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                CreatedTimestamp = (long)(document.CreatedDate - new DateTime(1970, 1, 1)).TotalSeconds
            }).SingleOrDefaultAsync();
        
        var dataJson = new { data = documentFromDb };
        return Ok(dataJson);
    }
    
    [HttpPost]
    public async Task<IActionResult>  GetDocumentsPage(DataTablesParameters parameters)
    {
        // Total count of documents
        var totalCount = await _dbContext.Documents.CountAsync();

        // Query for documents with join
        var query = from document in _dbContext.Documents
                    join department in _dbContext.Departments.AsNoTracking()
                        on document.DepartmentId equals department.Id
                    join requestType in _dbContext.RequestTypes.AsNoTracking()
                        on document.RequestTypeId equals requestType.Id
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
                    };

        // Apply paging
        var documents = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync();

        var dataJson = new
        {
            draw = parameters.Draw,
            recordsTotal = totalCount,
            recordsFiltered = totalCount, // We're not applying filtering in this example
            data = documents
        };

        return Ok(dataJson);
    
    }
 
    [HttpPost]
    public async Task<IActionResult>  CreateDocument([FromBody]DocumentVm documentVm)
    {
        try
        {
            Document document = new Document();
            document.CreatedDate = DateTime.Now;
            document.ModifiedDate = DateTime.Now;
       
            if (documentVm.RequestTypeId is null)
            {
                ModelState.AddModelError("RequestTypeId", "RequestType is required");
            }
            
            if (documentVm.RouteDepartmentId is null)
            {
                ModelState.AddModelError("RouteDepartmentId", "Route Department is required");
            }
        
            if (ModelState.IsValid)
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();
                
                document.Title = documentVm.Title;
                document.Content = documentVm.Content;
                document.TrackingCode = documentVm.TrackingCode;
                document.DepartmentId = employee.DepartmentId;
                document.RequestTypeId = documentVm.RequestTypeId.Value;
                document.Remarks = documentVm.Remarks;
                document.RouteDepartmentId = documentVm.RouteDepartmentId;
                document.CreatedBy = employee.Id;
                document.ModifiedBy = employee.Id;
                
                //Status Id for forwarded is 1
                document.StatusId = (int)StatusEnum.Forwarded;;
            
                await _dbContext.Documents.AddAsync(document);
                await _dbContext.SaveChangesAsync();
                
                //Create Tracking History
                TrackingHistory transactionHistory = new TrackingHistory();
                transactionHistory.DepartmentId = employee.DepartmentId;
                transactionHistory.StatusId = (int)StatusEnum.Forwarded;
                transactionHistory.Title = documentVm.Title;
                transactionHistory.TrackingCode = documentVm.TrackingCode;
                transactionHistory.Content = documentVm.Content;
                transactionHistory.Remarks = documentVm.Remarks;
                transactionHistory.CreatedDate = DateTime.Now;
                transactionHistory.ModifiedDate = DateTime.Now;
                transactionHistory.CreatedBy = employee.Id;
                transactionHistory.ModifiedBy = employee.Id;
                transactionHistory.RouteDepartmentId = documentVm.RouteDepartmentId;
                transactionHistory.RequestTypeId = documentVm.RequestTypeId.Value;
                transactionHistory.DocumentId = document.Id;

                await _dbContext.TrackingHistories.AddAsync(transactionHistory);
                await _dbContext.SaveChangesAsync();
              
                var dataJson = new { isSuccess = true };
            
                return Ok(dataJson);
            }
            else
            {
                var errors = ModelState.Keys
                    .Select(key => new
                    {
                        Field = key,
                        Error = ModelState[key].Errors.FirstOrDefault()?.ErrorMessage
                    })
                    .Where(item => item.Error != null)
                    .ToList();
              
                var dataJson = new { isSuccess = false, errors = errors };
                return BadRequest(dataJson);
            }
        }
        catch (Exception e)
        {
            var dataJson = new { isSuccess = false, errors = e.Message };
            return BadRequest(dataJson);
        }
       
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDocument([FromBody] DocumentVm documentVm)
    {
        try
        {
            Document document = await _dbContext.Documents.Where(doc => doc.Id == documentVm.Id).FirstOrDefaultAsync();
            document.ModifiedDate = DateTime.Now;
              
            if (documentVm.RequestTypeId is null)
            {
                ModelState.AddModelError("RequestTypeId", "RequestType is required");
            }

            if (documentVm.RouteDepartmentId is null)
            {
                ModelState.AddModelError("RouteDepartmentId", "Route Department is required");
            }
        
            if (ModelState.IsValid)
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var employee = await _dbContext.Employees.Where(e => e.UserId == userId).FirstOrDefaultAsync();
                
                document.Title = documentVm.Title;
                document.Content = documentVm.Content;
                document.TrackingCode = documentVm.TrackingCode;
                document.DepartmentId = employee.DepartmentId;
                document.RequestTypeId = documentVm.RequestTypeId.Value;
                document.Remarks = documentVm.Remarks;
                document.RouteDepartmentId = documentVm.RouteDepartmentId;
                document.ModifiedBy = employee.Id;
                if (document.StatusId != (int)StatusEnum.Received && document.StatusId != (int)StatusEnum.Completed)
                {
                    document.StatusId = (int)StatusEnum.Forwarded;
                }
                
                _dbContext.Documents.Update(document);
                await _dbContext.SaveChangesAsync();

                var dataJson = new { isSuccess = true };
            
                return Ok(dataJson);
            }
            else
            {
                var errors = ModelState.Keys
                    .Select(key => new
                    {
                        Field = key,
                        Error = ModelState[key].Errors.FirstOrDefault()?.ErrorMessage
                    })
                    .Where(item => item.Error != null)
                    .ToList();
              
                var dataJson = new { isSuccess = false, errors = errors };
                return BadRequest(dataJson);
            }
        }
        catch (Exception e)
        {
            var dataJson = new { isSuccess = false, errors = e.Message };
            return BadRequest(e.Message);
        }
        
        

    }
    
    [HttpPost]
    public IActionResult GetAllDocuments()
{
    try
    {
        var draw = Request.Form["draw"].FirstOrDefault();
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
        var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
        var searchValue = Request.Form["search[value]"].FirstOrDefault();

        int pageSize = length != null ? Convert.ToInt32(length) : 0;
        int skip = start != null ? Convert.ToInt32(start) : 0;
        int recordsTotal = 0;

        // Log the received parameters for debugging
        _logger.LogInformation($"Draw: {draw}, Start: {start}, Length: {length}, SortColumn: {sortColumn}, SortDirection: {sortColumnDirection}, SearchValue: {searchValue}");

        var customerData = from tempcustomer in _dbContext.Documents select tempcustomer;

        if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
        {
            customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
        }

        if (!string.IsNullOrEmpty(searchValue))
        {
            customerData = customerData.Where(m => m.Title.Contains(searchValue)
                                                   || m.Remarks.Contains(searchValue)
                                                   || m.Content.Contains(searchValue));

        }

        recordsTotal = customerData.Count();

        var data = customerData.Skip(skip).Take(pageSize).ToList();
        
        _logger.LogInformation($"Retrieved Data: {data.Count}");

        var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };

        return Ok(jsonData);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while fetching customer data.");
        return StatusCode(500, "Internal server error");
    }
}



    #endregion
    
}