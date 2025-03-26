using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    /// <inheritdoc />
    public partial class help : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TypesSubjects",
                table: "TypesSubjects");

            migrationBuilder.RenameColumn(
                name: "SubjectsIdsubjects",
                table: "TypesSubjects",
                newName: "subjects_idsubjects");

            migrationBuilder.AlterColumn<int>(
                name: "TypesId",
                table: "TypesSubjects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<int>(
                name: "types_id",
                table: "TypesSubjects",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypesSubjects",
                table: "TypesSubjects",
                columns: new[] { "types_id", "subjects_idsubjects" });

            migrationBuilder.CreateIndex(
                name: "IX_TypesSubjects_TypesId",
                table: "TypesSubjects",
                column: "TypesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TypesSubjects",
                table: "TypesSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TypesSubjects_TypesId",
                table: "TypesSubjects");

            migrationBuilder.DropColumn(
                name: "types_id",
                table: "TypesSubjects");

            migrationBuilder.RenameColumn(
                name: "subjects_idsubjects",
                table: "TypesSubjects",
                newName: "SubjectsIdsubjects");

            migrationBuilder.AlterColumn<int>(
                name: "TypesId",
                table: "TypesSubjects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypesSubjects",
                table: "TypesSubjects",
                columns: new[] { "TypesId", "SubjectsIdsubjects" });
        }
    }
}
