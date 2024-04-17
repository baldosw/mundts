using DTS.Common;
using DTS.Common.DataTables;
using DTS.DataAccess;
using DTS.Models;
using DTS.Web.Areas.User.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DTS.Web.Controllers;

[Area("User")]
public class DocumentController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    public DocumentController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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
           
        return View(documentVm);
    }
    
    public IActionResult Create()
    {
        DocumentVm documentVm = new DocumentVm();
        documentVm.TrackingCode = TrackingCodeGenerator.Generate();

        documentVm.RequestTypes = _dbContext.RequestTypes.Select(rt => new SelectListItem
        {
            Value = rt.Id.ToString(),
            Text = rt.Title
        });
         
        return View(documentVm);
    }


    #region Documents Api

    [HttpGet]
    public async Task<IActionResult>  GetDocuments()
    {
        var documents = await (from document in _dbContext.Documents
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
            document.CreatedBy = 3;
            document.ModifiedBy = 3;

            if (documentVm.DepartmentId is null)
            {
                ModelState.AddModelError("DepartmentId", "Department is required");
            }
        
            if (documentVm.RequestTypeId is null)
            {
                ModelState.AddModelError("RequestTypeId", "RequestType is required");
            }
        
            if (ModelState.IsValid)
            {
                document.Title = documentVm.Title;
                document.Content = documentVm.Content;
                document.TrackingCode = documentVm.TrackingCode;
                document.DepartmentId = documentVm.DepartmentId.Value;
                document.RequestTypeId = documentVm.RequestTypeId.Value;
                document.Remarks = documentVm.Remarks;
                document.RouteDepartmentId = documentVm.RouteDepartmentId;
                
                //Status Id for forwarded is 1
                document.StatusId = (int)StatusEnum.Forwarded;;
            
                await _dbContext.Documents.AddAsync(document);
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
            document.CreatedBy = 3;
            document.ModifiedBy = 3;

            if (documentVm.DepartmentId is null)
            {
                ModelState.AddModelError("DepartmentId", "Department is required");
            }
        
            if (documentVm.RequestTypeId is null)
            {
                ModelState.AddModelError("RequestTypeId", "RequestType is required");
            }
        
            if (ModelState.IsValid)
            {
                document.Title = documentVm.Title;
                document.Content = documentVm.Content;
                document.TrackingCode = documentVm.TrackingCode;
                document.DepartmentId = documentVm.DepartmentId.Value;
                document.RequestTypeId = documentVm.RequestTypeId.Value;
                document.Remarks = documentVm.Remarks;
                document.RouteDepartmentId = documentVm.RouteDepartmentId;
            
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


    #endregion
    
}