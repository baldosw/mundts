 
using DTS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using DTS.DataAccess;

namespace DataTableSampleMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<CustomerController> logger;

        public CustomerController(ApplicationDbContext context, ILogger<CustomerController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

       [HttpPost]
public IActionResult GetCustomers()
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
        logger.LogInformation($"Draw: {draw}, Start: {start}, Length: {length}, SortColumn: {sortColumn}, SortDirection: {sortColumnDirection}, SearchValue: {searchValue}");

        var customerData = from tempcustomer in context.Documents select tempcustomer;

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
        
        logger.LogInformation($"Retrieved Data: {data.Count}");

        var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };

        return Ok(jsonData);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while fetching customer data.");
        return StatusCode(500, "Internal server error");
    }
}


    }

}
