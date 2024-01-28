using Microsoft.EntityFrameworkCore.Migrations;

namespace AlobarShopp.Migrations
{
    public partial class AddPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CartName",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cvc",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpiriationMonth",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpiriationYear",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CartName",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "Cvc",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ExpiriationMonth",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ExpiriationYear",
                table: "OrderHeaders");
        }
    }
}
