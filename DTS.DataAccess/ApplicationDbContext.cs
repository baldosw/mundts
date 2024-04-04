using DTS.Models;
using Microsoft.EntityFrameworkCore;

namespace DTS.DataAccess;
  
public partial class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Status> Statuses { get; set; }
    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<RequestType> RequestTypes { get; set; }
}