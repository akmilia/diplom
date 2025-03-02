using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplom.Migrations
{
    /// <inheritdoc />
    public partial class sixth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "types_subjects_subjects_idsubjects_fkey",
                table: "types_subjects");

            migrationBuilder.DropForeignKey(
                name: "types_subjects_types_id_fkey",
                table: "types_subjects");

            migrationBuilder.DropIndex(
                name: "IX_types_subjects_subjects_idsubjects",
                table: "types_subjects");

            migrationBuilder.DropIndex(
                name: "IX_types_subjects_types_id",
                table: "types_subjects");

            migrationBuilder.RenameTable(
                name: "types_subjects",
                newName: "TypesSubjects");

            migrationBuilder.RenameColumn(
                name: "types_id",
                table: "TypesSubjects",
                newName: "TypesId");

            migrationBuilder.RenameColumn(
                name: "subjects_idsubjects",
                table: "TypesSubjects",
                newName: "SubjectsIdsubjects");

            migrationBuilder.AddColumn<int>(
                name: "SubjectsIdsubjectsNavigationIdsubjects",
                table: "TypesSubjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypesSubjects",
                table: "TypesSubjects",
                columns: new[] { "TypesId", "SubjectsIdsubjects" });

            migrationBuilder.CreateIndex(
                name: "IX_TypesSubjects_SubjectsIdsubjectsNavigationIdsubjects",
                table: "TypesSubjects",
                column: "SubjectsIdsubjectsNavigationIdsubjects");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypesSubjects_subjects_SubjectsIdsubjectsNavigationIdsubjec~",
                table: "TypesSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TypesSubjects_types_TypesId",
                table: "TypesSubjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypesSubjects",
                table: "TypesSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TypesSubjects_SubjectsIdsubjectsNavigationIdsubjects",
                table: "TypesSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectsIdsubjectsNavigationIdsubjects",
                table: "TypesSubjects");

            migrationBuilder.RenameTable(
                name: "TypesSubjects",
                newName: "types_subjects");

            migrationBuilder.RenameColumn(
                name: "SubjectsIdsubjects",
                table: "types_subjects",
                newName: "subjects_idsubjects");

            migrationBuilder.RenameColumn(
                name: "TypesId",
                table: "types_subjects",
                newName: "types_id");

            migrationBuilder.CreateIndex(
                name: "IX_types_subjects_subjects_idsubjects",
                table: "types_subjects",
                column: "subjects_idsubjects");

            migrationBuilder.CreateIndex(
                name: "IX_types_subjects_types_id",
                table: "types_subjects",
                column: "types_id");

            migrationBuilder.AddForeignKey(
                name: "types_subjects_subjects_idsubjects_fkey",
                table: "types_subjects",
                column: "subjects_idsubjects",
                principalTable: "subjects",
                principalColumn: "idsubjects");

            migrationBuilder.AddForeignKey(
                name: "types_subjects_types_id_fkey",
                table: "types_subjects",
                column: "types_id",
                principalTable: "types",
                principalColumn: "id");
        }
    }
}
