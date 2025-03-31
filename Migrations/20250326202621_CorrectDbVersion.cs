using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    /// <inheritdoc />
    public partial class CorrectDbVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypesSubjects_subjects_SubjectsIdsubjectsNavigationIdsubjec~",
                table: "TypesSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TypesSubjects_types_TypesId",
                table: "TypesSubjects");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_users_idusers",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "users_pkey",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypesSubjects",
                table: "TypesSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TypesSubjects_SubjectsIdsubjectsNavigationIdsubjects",
                table: "TypesSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TypesSubjects_TypesId",
                table: "TypesSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectsIdsubjectsNavigationIdsubjects",
                table: "TypesSubjects");

            migrationBuilder.DropColumn(
                name: "TypesId",
                table: "TypesSubjects");

            migrationBuilder.RenameTable(
                name: "TypesSubjects",
                newName: "types_subjects");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "time",
                table: "schedule",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "time with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date",
                table: "attendance",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddPrimaryKey(
                name: "users_pkey",
                table: "users",
                column: "idusers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_types_subjects",
                table: "types_subjects",
                columns: new[] { "types_id", "subjects_idsubjects" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "users_pkey",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_types_subjects",
                table: "types_subjects");

            migrationBuilder.RenameTable(
                name: "types_subjects",
                newName: "TypesSubjects");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "time",
                table: "schedule",
                type: "time with time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "date",
                table: "attendance",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<int>(
                name: "SubjectsIdsubjectsNavigationIdsubjects",
                table: "TypesSubjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypesId",
                table: "TypesSubjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_users_idusers",
                table: "users",
                column: "idusers");

            migrationBuilder.AddPrimaryKey(
                name: "users_pkey",
                table: "users",
                columns: new[] { "idusers", "login", "roles_idroles" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypesSubjects",
                table: "TypesSubjects",
                columns: new[] { "types_id", "subjects_idsubjects" });

            migrationBuilder.CreateIndex(
                name: "IX_TypesSubjects_SubjectsIdsubjectsNavigationIdsubjects",
                table: "TypesSubjects",
                column: "SubjectsIdsubjectsNavigationIdsubjects");

            migrationBuilder.CreateIndex(
                name: "IX_TypesSubjects_TypesId",
                table: "TypesSubjects",
                column: "TypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_TypesSubjects_subjects_SubjectsIdsubjectsNavigationIdsubjec~",
                table: "TypesSubjects",
                column: "SubjectsIdsubjectsNavigationIdsubjects",
                principalTable: "subjects",
                principalColumn: "idsubjects",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TypesSubjects_types_TypesId",
                table: "TypesSubjects",
                column: "TypesId",
                principalTable: "types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
