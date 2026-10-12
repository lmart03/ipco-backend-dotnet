using Ipco.Modules.Patients.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ipco.Modules.Patients.Infrastructure;

internal sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients", table =>
        {
            table.HasCheckConstraint("ck_patients_document_complete", "(document_type_id IS NULL) = (document_number IS NULL)");
            table.HasCheckConstraint("ck_patients_phone_format", "phone ~ '^\\+?[0-9]{6,15}$'");
            table.HasCheckConstraint("ck_patients_email_normalized", "email IS NULL OR email = lower(btrim(email))");
        });
        builder.HasKey(x => x.Id).HasName("pk_patients");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.DocumentTypeId).HasColumnName("document_type_id");
        builder.Property(x => x.DocumentNumber).HasColumnName("document_number").HasMaxLength(Patient.DocumentNumberMaxLength);
        builder.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(Patient.NameMaxLength).IsRequired();
        builder.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(Patient.NameMaxLength).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(Patient.EmailMaxLength);
        builder.Property(x => x.BirthDate).HasColumnName("birth_date");
        builder.Property(x => x.GenderId).HasColumnName("gender_id");
        builder.Property(x => x.MaritalStatusId).HasColumnName("marital_status_id");
        builder.Property(x => x.Address).HasColumnName("address").HasMaxLength(Patient.AddressMaxLength);
        builder.Property(x => x.EmergencyContactName).HasColumnName("emergency_contact_name").HasMaxLength(Patient.EmergencyContactNameMaxLength);
        builder.Property(x => x.EmergencyContactPhone).HasColumnName("emergency_contact_phone").HasMaxLength(16);
        builder.Property(x => x.Allergies).HasColumnName("allergies").HasMaxLength(Patient.AllergiesMaxLength);
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("deleted").IsRequired();
        builder.Property(x => x.DeletedAtUtc).HasColumnName("deleted_at");
        builder.Property<uint>("Version").IsRowVersion();
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => new { x.LastName, x.FirstName }).HasDatabaseName("ix_patients_name");
        builder.HasIndex(x => x.Phone).HasDatabaseName("ix_patients_phone");
        builder.HasIndex(x => new { x.DocumentTypeId, x.DocumentNumber }).IsUnique()
            .HasFilter("deleted = FALSE AND document_number IS NOT NULL")
            .HasDatabaseName("uk_patients_document_active");
    }
}
