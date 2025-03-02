using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace diplom.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cabinets",
                columns: table => new
                {
                    idcabinet = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cabinets_pkey", x => x.idcabinet);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    idgroups = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("groups_pkey", x => x.idgroups);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    idroles = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("roles_pkey", x => x.idroles);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    idsubjects = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjects_pkey", x => x.idsubjects);
                });

            migrationBuilder.CreateTable(
                name: "types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    idusers = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    roles_idroles = table.Column<int>(type: "integer", nullable: false),
                    surname = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    paternity = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: true),
                    password = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => new { x.idusers, x.login, x.roles_idroles });
                    table.UniqueConstraint("AK_users_idusers", x => x.idusers);
                    table.ForeignKey(
                        name: "users_roles_idroles_fkey",
                        column: x => x.roles_idroles,
                        principalTable: "roles",
                        principalColumn: "idroles");
                });

            migrationBuilder.CreateTable(
                name: "types_subjects",
                columns: table => new
                {
                    types_id = table.Column<int>(type: "integer", nullable: false),
                    subjects_idsubjects = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "types_subjects_subjects_idsubjects_fkey",
                        column: x => x.subjects_idsubjects,
                        principalTable: "subjects",
                        principalColumn: "idsubjects");
                    table.ForeignKey(
                        name: "types_subjects_types_id_fkey",
                        column: x => x.types_id,
                        principalTable: "types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "groups_users",
                columns: table => new
                {
                    groups_idgroups = table.Column<int>(type: "integer", nullable: false),
                    users_idusers = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "groups_users_groups_idgroups_fkey",
                        column: x => x.groups_idgroups,
                        principalTable: "groups",
                        principalColumn: "idgroups");
                    table.ForeignKey(
                        name: "groups_users_users_idusers_fkey",
                        column: x => x.users_idusers,
                        principalTable: "users",
                        principalColumn: "idusers");
                });

            migrationBuilder.CreateTable(
                name: "schedule",
                columns: table => new
                {
                    idschedule = table.Column<int>(type: "integer", nullable: false),
                    time = table.Column<DateTimeOffset>(type: "time with time zone", nullable: false),
                    subjects_idsubjects = table.Column<int>(type: "integer", nullable: false),
                    users_idusers = table.Column<int>(type: "integer", nullable: false),
                    cabinets_idcabinet = table.Column<int>(type: "integer", nullable: false),
                    groups_idgroup = table.Column<int>(type: "integer", nullable: true),
                    day_of_week = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("schedule_pk", x => x.idschedule);
                    table.ForeignKey(
                        name: "schedule_cabinets_idcabinet_fkey",
                        column: x => x.cabinets_idcabinet,
                        principalTable: "cabinets",
                        principalColumn: "idcabinet");
                    table.ForeignKey(
                        name: "schedule_group_idgroup_fkey",
                        column: x => x.groups_idgroup,
                        principalTable: "groups",
                        principalColumn: "idgroups");
                    table.ForeignKey(
                        name: "schedule_subjects_idsubjects_fkey",
                        column: x => x.subjects_idsubjects,
                        principalTable: "subjects",
                        principalColumn: "idsubjects");
                    table.ForeignKey(
                        name: "schedule_users_idusers_fkey",
                        column: x => x.users_idusers,
                        principalTable: "users",
                        principalColumn: "idusers");
                });

            migrationBuilder.CreateTable(
                name: "attendance",
                columns: table => new
                {
                    idattendance = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idschedule = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("attendance_pkey", x => x.idattendance);
                    table.ForeignKey(
                        name: "attendance_schedule_idschedule",
                        column: x => x.idschedule,
                        principalTable: "schedule",
                        principalColumn: "idschedule");
                });

            migrationBuilder.CreateTable(
                name: "bil_nebil",
                columns: table => new
                {
                    idattendance = table.Column<int>(type: "integer", nullable: false),
                    iduser = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bil_nebil_pkey", x => new { x.idattendance, x.iduser });
                    table.ForeignKey(
                        name: "bil_attendance_idattendance",
                        column: x => x.idattendance,
                        principalTable: "attendance",
                        principalColumn: "idattendance");
                });

            migrationBuilder.CreateIndex(
                name: "IX_attendance_idschedule",
                table: "attendance",
                column: "idschedule");

            migrationBuilder.CreateIndex(
                name: "IX_groups_users_groups_idgroups",
                table: "groups_users",
                column: "groups_idgroups");

            migrationBuilder.CreateIndex(
                name: "IX_groups_users_users_idusers",
                table: "groups_users",
                column: "users_idusers");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_cabinets_idcabinet",
                table: "schedule",
                column: "cabinets_idcabinet");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_groups_idgroup",
                table: "schedule",
                column: "groups_idgroup");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_subjects_idsubjects",
                table: "schedule",
                column: "subjects_idsubjects");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_users_idusers",
                table: "schedule",
                column: "users_idusers");

            migrationBuilder.CreateIndex(
                name: "IX_types_subjects_subjects_idsubjects",
                table: "types_subjects",
                column: "subjects_idsubjects");

            migrationBuilder.CreateIndex(
                name: "IX_types_subjects_types_id",
                table: "types_subjects",
                column: "types_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_roles_idroles",
                table: "users",
                column: "roles_idroles");

            migrationBuilder.CreateIndex(
                name: "users_idusers_key",
                table: "users",
                column: "idusers",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_login_key",
                table: "users",
                column: "login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bil_nebil");

            migrationBuilder.DropTable(
                name: "groups_users");

            migrationBuilder.DropTable(
                name: "types_subjects");

            migrationBuilder.DropTable(
                name: "attendance");

            migrationBuilder.DropTable(
                name: "types");

            migrationBuilder.DropTable(
                name: "schedule");

            migrationBuilder.DropTable(
                name: "cabinets");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
