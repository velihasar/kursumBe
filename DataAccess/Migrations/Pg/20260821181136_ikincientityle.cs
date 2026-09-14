using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class ikincientityle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Translates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Translates",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Translates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Translates",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Translates",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Translates",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Translates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Translates",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "OperationClaims",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OperationClaims",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "OperationClaims",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "OperationClaims",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OperationClaims",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OperationClaims",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "OperationClaims",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "OperationClaims",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "MobileLogins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MobileLogins",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "MobileLogins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "MobileLogins",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MobileLogins",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MobileLogins",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "MobileLogins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "MobileLogins",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Languages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Languages",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Languages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Languages",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Languages",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Languages",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Languages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Languages",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Groups",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Groups",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Groups",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Groups",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Groups",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Groups",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Groups",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Groups",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BranchId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUsers_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenantUsers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parents_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Parents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    StudentNumber = table.Column<string>(type: "text", nullable: true),
                    EnrollmentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teachers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentParents",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: false),
                    Relationship = table.Column<string>(type: "text", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentParents", x => new { x.StudentId, x.ParentId });
                    table.ForeignKey(
                        name: "FK_StudentParents_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentParents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentParents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 133,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 134,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 135,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 136,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 137,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.UpdateData(
                table: "Translates",
                keyColumn: "Id",
                keyValue: 138,
                columns: new[] { "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsActive", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, null, null, true, false, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_TenantId",
                table: "Branches",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Parents_PersonId",
                table: "Parents",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parents_TenantId",
                table: "Parents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_People_TenantId",
                table: "People",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentParents_ParentId",
                table: "StudentParents",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentParents_TenantId",
                table: "StudentParents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_PersonId",
                table: "Students",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_TenantId",
                table: "Students",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_PersonId",
                table: "Teachers",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TenantId",
                table: "Teachers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_BranchId",
                table: "TenantUsers",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_TenantId",
                table: "TenantUsers",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentParents");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "TenantUsers");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Translates");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Translates");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Translates");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Translates");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Translates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Translates");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Translates");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Translates");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "OperationClaims");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MobileLogins");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MobileLogins");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "MobileLogins");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "MobileLogins");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MobileLogins");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MobileLogins");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MobileLogins");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MobileLogins");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Groups");
        }
    }
}
