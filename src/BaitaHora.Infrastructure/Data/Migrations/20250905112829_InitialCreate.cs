using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaitaHora.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    addr_street = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    addr_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    addr_complement = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    addr_neighborhood = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    addr_city = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    addr_state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    addr_zip_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "citext", maxLength: 256, nullable: false),
                    trade_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    name = table.Column<string>(type: "citext", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    customer_phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    customer_cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "login_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    RefreshTokenExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    Ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    OccurredOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StoredOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PublishedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    NextAttemptUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    LockToken = table.Column<Guid>(type: "uuid", nullable: true),
                    LockedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    rg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    UserPhone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Address_Street = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Address_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address_Complement = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Address_Neighborhood = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Address_City = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Address_State = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Address_ZipCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    profile_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "company_positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_mask = table.Column<int>(type: "integer", nullable: false),
                    access_level = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    position_name = table.Column<string>(type: "citext", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_company_positions_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "company_service_offerings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    service_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_service_offerings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_company_service_offerings_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyImages_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    starts_at_utc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "Pending"),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.Id);
                    table.CheckConstraint("ck_appointments_duration_positive", "duration > interval '0 seconds'");
                    table.CheckConstraint("ck_appointments_status_valid", "status in ('Pending','Cancelled','Completed')");
                    table.ForeignKey(
                        name: "FK_appointments_schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "citext", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    pwd_reset_token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    pwd_reset_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    token_version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_user_profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "user_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "company_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    primary_position_id = table.Column<Guid>(type: "uuid", nullable: true),
                    direct_permission_mask = table.Column<int>(type: "integer", nullable: false),
                    joined_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_company_members_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_company_members_company_positions_primary_position_id",
                        column: x => x.primary_position_id,
                        principalTable: "company_positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "company_position_service_offerings",
                columns: table => new
                {
                    position_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_service_offering_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_position_service_offerings", x => new { x.position_id, x.company_service_offering_id });
                    table.ForeignKey(
                        name: "FK_company_position_service_offerings_company_positions_positi~",
                        column: x => x.position_id,
                        principalTable: "company_positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_company_position_service_offerings_company_service_offering~",
                        column: x => x.company_service_offering_id,
                        principalTable: "company_service_offerings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_schedule",
                table: "appointments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "ux_appointments_schedule_start",
                table: "appointments",
                columns: new[] { "ScheduleId", "starts_at_utc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_companies_cnpj",
                table: "companies",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_companies_email",
                table: "companies",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_companies_name",
                table: "companies",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_company_members_company",
                table: "company_members",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_company_members_primary_position",
                table: "company_members",
                column: "primary_position_id");

            migrationBuilder.CreateIndex(
                name: "ix_company_members_user",
                table: "company_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_company_members_company_user",
                table: "company_members",
                columns: new[] { "company_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cpso_company_service",
                table: "company_position_service_offerings",
                column: "company_service_offering_id");

            migrationBuilder.CreateIndex(
                name: "ix_cpso_position",
                table: "company_position_service_offerings",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "ix_company_positions_companyid",
                table: "company_positions",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ux_company_positions_companyid_name",
                table: "company_positions",
                columns: new[] { "company_id", "position_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cso_company",
                table: "company_service_offerings",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ux_cso_company_name",
                table: "company_service_offerings",
                columns: new[] { "company_id", "service_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyImages_CompanyId",
                table: "CompanyImages",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customers_name",
                table: "customers",
                column: "customer_name");

            migrationBuilder.CreateIndex(
                name: "ux_customers_cpf",
                table: "customers",
                column: "customer_cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_customers_phone",
                table: "customers",
                column: "customer_phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_login_sessions_rthash",
                table: "login_sessions",
                column: "RefreshTokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_login_sessions_UserId",
                table: "login_sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_login_sessions_UserId_IsRevoked",
                table: "login_sessions",
                columns: new[] { "UserId", "IsRevoked" });

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_Status_NextAttemptUtc",
                table: "outbox_messages",
                columns: new[] { "Status", "NextAttemptUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_StoredOnUtc",
                table: "outbox_messages",
                column: "StoredOnUtc");

            migrationBuilder.CreateIndex(
                name: "ux_schedules_user",
                table: "schedules",
                column: "MemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_user_profiles_cpf",
                table: "user_profiles",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_user_profiles_phone",
                table: "user_profiles",
                column: "UserPhone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_user_profiles_rg",
                table: "user_profiles",
                column: "rg",
                unique: true,
                filter: "\"rg\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_ProfileId",
                table: "users",
                column: "ProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "company_members");

            migrationBuilder.DropTable(
                name: "company_position_service_offerings");

            migrationBuilder.DropTable(
                name: "CompanyImages");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "login_sessions");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "company_positions");

            migrationBuilder.DropTable(
                name: "company_service_offerings");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
