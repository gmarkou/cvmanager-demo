using CVManager.DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace CVManager.DAL;

public class DataContext: DbContext 
{
    public DataContext(DbContextOptions options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cv>()
            .Property(cv => cv.DateCreated)
            .HasDefaultValueSql("now()");
    }
    
    public DbSet<Cv>? Cvs { get; set; }
    public DbSet<Degree>? Degrees { get; set; }
}