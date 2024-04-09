using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doan_Web_CK.Migrations
{
    /// <inheritdoc />
    public partial class Add_ConnectioRoomCall_To_Message : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "connectionRoomCall",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "connectionRoomCall",
                table: "Messages");
        }
    }
}
