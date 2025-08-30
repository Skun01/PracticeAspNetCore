using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearnWebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitDataase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Laptop Dell XPS 15", 45000000m },
                    { 2, "Bàn phím cơ Logitech G Pro", 3500000m },
                    { 3, "Chuột gaming Razer DeathAdder V2", 1800000m },
                    { 4, "Màn hình cong Samsung Odyssey G9", 28000000m },
                    { 5, "Tai nghe chống ồn Sony WH-1000XM5", 8000000m },
                    { 6, "Ổ cứng SSD Samsung 980 Pro 1TB", 3200000m },
                    { 7, "Máy ảnh Fujifilm X-T5", 38000000m },
                    { 8, "Ống kính Canon RF 50mm f/1.8", 6000000m },
                    { 9, "Bếp điện từ Kangaroo KG499I", 1500000m },
                    { 10, "Nồi chiên không dầu Philips HD9252/90", 2800000m },
                    { 11, "Robot hút bụi Xiaomi Robot Vacuum E10", 5500000m },
                    { 12, "Tivi Sony Bravia 55 inch", 17500000m },
                    { 13, "Loa Bluetooth JBL Flip 6", 2000000m },
                    { 14, "Máy lọc không khí Coway AP-1009CH", 4200000m },
                    { 15, "Đồng hồ thông minh Apple Watch Series 8", 12000000m },
                    { 16, "Giày thể thao Adidas Ultraboost 22", 3000000m },
                    { 17, "Áo khoác gió Uniqlo", 800000m },
                    { 18, "Sách 'Đắc nhân tâm'", 120000m },
                    { 19, "Dầu gội đầu Rejoice", 85000m },
                    { 20, "Bộ LEGO Technic Bugatti Chiron", 10500000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
