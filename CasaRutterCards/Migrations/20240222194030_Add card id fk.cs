using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaRutterCards.Migrations
{
    /// <inheritdoc />
    public partial class Addcardidfk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Cards_CardId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "CardId",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Cards_CardId",
                table: "Items",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Cards_CardId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "CardId",
                table: "Items",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Cards_CardId",
                table: "Items",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id");
        }
    }
}
