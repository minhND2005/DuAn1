using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fast_Food.Migrations
{
    /// <inheritdoc />
    public partial class hi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    maKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenKhachHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gioiTinh = table.Column<bool>(type: "bit", nullable: true),
                    soDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ngaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    diaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    avatar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__0CCB3D49D960668A", x => x.maKhachHang);
                });

            migrationBuilder.CreateTable(
                name: "MonAn",
                columns: table => new
                {
                    maMon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    loaiSanPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    tenMon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    trangThai = table.Column<bool>(type: "bit", nullable: true),
                    ngayTao = table.Column<DateOnly>(type: "date", nullable: true),
                    ngayCapNhat = table.Column<DateOnly>(type: "date", nullable: true),
                    chiTietFood = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MonAn__27547BFA7EFB4EBB", x => x.maMon);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    maNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gioiTinh = table.Column<bool>(type: "bit", nullable: true),
                    CCCD = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    soDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ngaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    diaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    tuoi = table.Column<int>(type: "int", nullable: true),
                    avatar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhanVien__BDDEF20DF78D35B1", x => x.maNhanVien);
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    maVoucher = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    soTienGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ngayBatDau = table.Column<DateOnly>(type: "date", nullable: false),
                    ngayKetThuc = table.Column<DateOnly>(type: "date", nullable: false),
                    trangThai = table.Column<bool>(type: "bit", nullable: true),
                    soLanSuDungToiDa = table.Column<int>(type: "int", nullable: false),
                    soLanSuDungHienTai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Voucher__E335C400381BF7D9", x => x.maVoucher);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietCombo",
                columns: table => new
                {
                    maCombo = table.Column<int>(type: "int", nullable: false),
                    maMon = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietC__7D763253D10AE6C3", x => new { x.maCombo, x.maMon });
                    table.ForeignKey(
                        name: "FK__ChiTietCo__maMon__2C3393D0",
                        column: x => x.maMon,
                        principalTable: "MonAn",
                        principalColumn: "maMon");
                });

            migrationBuilder.CreateTable(
                name: "gioHang",
                columns: table => new
                {
                    maGioHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maKhachHang = table.Column<int>(type: "int", nullable: false),
                    maMon = table.Column<int>(type: "int", nullable: true),
                    maCombo = table.Column<int>(type: "int", nullable: true),
                    tenSanPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    gia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    soLuong = table.Column<int>(type: "int", nullable: true),
                    ghiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    anh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__gioHang__2C76D203DE1916BA", x => x.maGioHang);
                    table.ForeignKey(
                        name: "FK__gioHang__maCombo__412EB0B6",
                        column: x => x.maCombo,
                        principalTable: "MonAn",
                        principalColumn: "maMon");
                    table.ForeignKey(
                        name: "FK__gioHang__maKhach__3F466844",
                        column: x => x.maKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "maKhachHang");
                    table.ForeignKey(
                        name: "FK__gioHang__maMon__403A8C7D",
                        column: x => x.maMon,
                        principalTable: "MonAn",
                        principalColumn: "maMon");
                });

            migrationBuilder.CreateTable(
                name: "taiKhoan",
                columns: table => new
                {
                    maTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenTK = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    matKhau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maKhachHang = table.Column<int>(type: "int", nullable: true),
                    maNhanVien = table.Column<int>(type: "int", nullable: true),
                    loaiTaiKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__taiKhoan__8FFF6A9D576AD94F", x => x.maTaiKhoan);
                    table.ForeignKey(
                        name: "FK__taiKhoan__maKhac__3B75D760",
                        column: x => x.maKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "maKhachHang");
                    table.ForeignKey(
                        name: "FK__taiKhoan__maNhan__3C69FB99",
                        column: x => x.maNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "maNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    maHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maKhachHang = table.Column<int>(type: "int", nullable: false),
                    maNhanVien = table.Column<int>(type: "int", nullable: true),
                    thoiGianDat = table.Column<DateTime>(type: "datetime", nullable: false),
                    thoiGianKetThuc = table.Column<DateTime>(type: "datetime", nullable: true),
                    trangThaiDonHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    trangThaiThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SDTLienHe = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    diaChiGiaoHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    danhGia = table.Column<double>(type: "float", nullable: false),
                    maVoucher = table.Column<int>(type: "int", nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HoaDon__026B4D9AD4E03AD1", x => x.maHoaDon);
                    table.ForeignKey(
                        name: "FK__HoaDon__maKhachH__300424B4",
                        column: x => x.maKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "maKhachHang");
                    table.ForeignKey(
                        name: "FK__HoaDon__maNhanVi__30F848ED",
                        column: x => x.maNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "maNhanVien");
                    table.ForeignKey(
                        name: "FK__HoaDon__maVouche__31EC6D26",
                        column: x => x.maVoucher,
                        principalTable: "Voucher",
                        principalColumn: "maVoucher");
                });

            migrationBuilder.CreateTable(
                name: "chiTietHoaDon",
                columns: table => new
                {
                    maChiTietHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maMon = table.Column<int>(type: "int", nullable: true),
                    maHoaDon = table.Column<int>(type: "int", nullable: false),
                    tenMonAn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    soLuong = table.Column<int>(type: "int", nullable: true),
                    gia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    tongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ghiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__chiTietH__7AF692ECEDC9F1F0", x => x.maChiTietHoaDon);
                    table.ForeignKey(
                        name: "FK__chiTietHo__maHoa__35BCFE0A",
                        column: x => x.maHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "maHoaDon");
                    table.ForeignKey(
                        name: "FK__chiTietHo__maMon__34C8D9D1",
                        column: x => x.maMon,
                        principalTable: "MonAn",
                        principalColumn: "maMon");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietCombo_maMon",
                table: "ChiTietCombo",
                column: "maMon");

            migrationBuilder.CreateIndex(
                name: "IX_chiTietHoaDon_maHoaDon",
                table: "chiTietHoaDon",
                column: "maHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_chiTietHoaDon_maMon",
                table: "chiTietHoaDon",
                column: "maMon");

            migrationBuilder.CreateIndex(
                name: "IX_gioHang_maCombo",
                table: "gioHang",
                column: "maCombo");

            migrationBuilder.CreateIndex(
                name: "IX_gioHang_maKhachHang",
                table: "gioHang",
                column: "maKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_gioHang_maMon",
                table: "gioHang",
                column: "maMon");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_maKhachHang",
                table: "HoaDon",
                column: "maKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_maNhanVien",
                table: "HoaDon",
                column: "maNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_maVoucher",
                table: "HoaDon",
                column: "maVoucher");

            migrationBuilder.CreateIndex(
                name: "UQ_KhachHang",
                table: "taiKhoan",
                column: "maKhachHang",
                unique: true,
                filter: "[maKhachHang] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_NhanVien",
                table: "taiKhoan",
                column: "maNhanVien",
                unique: true,
                filter: "[maNhanVien] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietCombo");

            migrationBuilder.DropTable(
                name: "chiTietHoaDon");

            migrationBuilder.DropTable(
                name: "gioHang");

            migrationBuilder.DropTable(
                name: "taiKhoan");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "MonAn");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "Voucher");
        }
    }
}
