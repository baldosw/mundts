using DTS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DTS.DataAccess;
  
public partial class ApplicationDbContext : IdentityDbContext<IdentityUser>
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
    
    public virtual DbSet<TrackingHistory> TrackingHistories { get; set; }
}