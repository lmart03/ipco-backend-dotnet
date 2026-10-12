using Microsoft.EntityFrameworkCore.Migrations;

namespace Ipco.Modules.Patients.Infrastructure.Migrations;

public sealed partial class InitialPatients : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "patients",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                document_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                document_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                gender_id = table.Column<Guid>(type: "uuid", nullable: true),
                marital_status_id = table.Column<Guid>(type: "uuid", nullable: true),
                address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                emergency_contact_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                emergency_contact_phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                allergies = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_patients", x => x.id);
                table.CheckConstraint("ck_patients_document_complete", "(document_type_id IS NULL) = (document_number IS NULL)");
                table.CheckConstraint("ck_patients_email_normalized", "email IS NULL OR email = lower(btrim(email))");
                table.CheckConstraint("ck_patients_phone_format", "phone ~ '^\\+?[0-9]{6,15}$'");
            });

        migrationBuilder.CreateIndex("ix_patients_name", "patients", new[] { "last_name", "first_name" });
        migrationBuilder.CreateIndex("ix_patients_phone", "patients", "phone");
        migrationBuilder.CreateIndex("uk_patients_document_active", "patients", new[] { "document_type_id", "document_number" },
            unique: true, filter: "deleted = FALSE AND document_number IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("patients");
}
