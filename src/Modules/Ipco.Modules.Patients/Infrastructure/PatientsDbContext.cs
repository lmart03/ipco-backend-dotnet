using Ipco.Modules.Patients.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ipco.Modules.Patients.Infrastructure;

public sealed class PatientsDbContext(DbContextOptions<PatientsDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientsDbContext).Assembly);
}
