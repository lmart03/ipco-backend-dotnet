using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ipco.Modules.Patients.Infrastructure.Migrations;

[DbContext(typeof(PatientsDbContext))]
internal sealed class PatientsDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.12");
        modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 63);
        modelBuilder.UseIdentityByDefaultColumns();
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
    }
}
