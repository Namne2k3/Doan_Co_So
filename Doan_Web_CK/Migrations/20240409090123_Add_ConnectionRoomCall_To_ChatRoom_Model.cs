using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doan_Web_CK.Migrations
{
    /// <inheritdoc />
    public partial class Add_ConnectionRoomCall_To_ChatRoom_Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionRoomCall",
                table: "chatRooms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionRoomCall",
                table: "chatRooms");
        }
    }
}
