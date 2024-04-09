using DTS.DataAccess;
using DTS.Models;
using DTS.Web.Areas.User.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DTS.Web.Controllers;

[Area("User")]
public class PersonalInformationController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    
    public PersonalInformationController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        PersonalInformationVm personalInformationVm = new PersonalInformationVm();

        personalInformationVm.Departments = await _dbContext.Departments.OrderBy(d => d.Name).AsNoTracking().Select(s =>
            new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).ToListAsync();

        Employee employee = await _dbContext.Employees.Include(e => e.Department).Where(e => e.Id == 3).SingleOrDefaultAsync();
        personalInformationVm.Employee = employee; 
        return View(personalInformationVm);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdatePersonalInformation(PersonalInformationVm personalInformationVm)
    {
        Employee employee = await _dbContext.Employees.Where(e => e.Id == personalInformationVm.Employee.Id).SingleOrDefaultAsync();

        if (employee is not null)
        {
            if (ModelState.IsValid)
            {
                employee.FirstName = personalInformationVm.Employee.FirstName;
                employee.MiddleName = personalInformationVm.Employee.MiddleName;
                employee.LastName = personalInformationVm.Employee.LastName;
                employee.Position = personalInformationVm.Employee.Position;
                employee.Email = personalInformationVm.Employee.Email;
                employee.Address = personalInformationVm.Employee.Address;
                employee.ContactNumber = personalInformationVm.Employee.ContactNumber;
                employee.DepartmentId = personalInformationVm.Employee.DepartmentId;

                _dbContext.Update(employee);
                await _dbContext.SaveChangesAsync();
            }
        }
        return RedirectToAction("Index");
    }
}