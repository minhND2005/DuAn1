using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Fast_Food.Models;

public partial class DoAnStoreContext : DbContext
{
    public DoAnStoreContext()
    {
    }

    public DoAnStoreContext(DbContextOptions<DoAnStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietCombo> ChiTietCombos { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<MonAn> MonAns { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MINH\\SQLEXPRESS;Database=DoAnStore;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietCombo>(entity =>
        {
            entity.HasKey(e => new { e.MaCombo, e.MaMon }).HasName("PK__ChiTietC__7D763253D10AE6C3");

            entity.ToTable("ChiTietCombo");

            entity.Property(e => e.MaCombo).HasColumnName("maCombo");
            entity.Property(e => e.MaMon).HasColumnName("maMon");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.ChiTietCombos)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietCo__maMon__2C3393D0");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaChiTietHoaDon).HasName("PK__chiTietH__7AF692ECEDC9F1F0");

            entity.ToTable("chiTietHoaDon");

            entity.Property(e => e.MaChiTietHoaDon).HasColumnName("maChiTietHoaDon");
            entity.Property(e => e.GhiChu).HasColumnName("ghiChu");
            entity.Property(e => e.Gia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia");
            entity.Property(e => e.MaHoaDon).HasColumnName("maHoaDon");
            entity.Property(e => e.MaMon).HasColumnName("maMon");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.TenMonAn)
                .HasMaxLength(50)
                .HasColumnName("tenMonAn");
            entity.Property(e => e.TongTien)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tongTien");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__chiTietHo__maHoa__35BCFE0A");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaMon)
                .HasConstraintName("FK__chiTietHo__maMon__34C8D9D1");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__gioHang__2C76D203DE1916BA");

            entity.ToTable("gioHang");

            entity.Property(e => e.MaGioHang).HasColumnName("maGioHang");
            entity.Property(e => e.GhiChu).HasColumnName("ghiChu");
            entity.Property(e => e.Gia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia");
            entity.Property(e => e.MaCombo).HasColumnName("maCombo");
            entity.Property(e => e.MaKhachHang).HasColumnName("maKhachHang");
            entity.Property(e => e.MaMon).HasColumnName("maMon");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.TenSanPham)
                .HasMaxLength(50)
                .HasColumnName("tenSanPham");

            entity.HasOne(d => d.MaComboNavigation).WithMany(p => p.GioHangMaComboNavigations)
                .HasForeignKey(d => d.MaCombo)
                .HasConstraintName("FK__gioHang__maCombo__412EB0B6");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__gioHang__maKhach__3F466844");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.GioHangMaMonNavigations)
                .HasForeignKey(d => d.MaMon)
                .HasConstraintName("FK__gioHang__maMon__403A8C7D");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__026B4D9AD4E03AD1");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon).HasColumnName("maHoaDon");
            entity.Property(e => e.DanhGia).HasColumnName("danhGia");
            entity.Property(e => e.DiaChiGiaoHang)
                .HasMaxLength(255)
                .HasColumnName("diaChiGiaoHang");
            entity.Property(e => e.MaKhachHang).HasColumnName("maKhachHang");
            entity.Property(e => e.MaNhanVien).HasColumnName("maNhanVien");
            entity.Property(e => e.MaVoucher).HasColumnName("maVoucher");
            entity.Property(e => e.SdtlienHe)
                .HasMaxLength(15)
                .HasColumnName("SDTLienHe");
            entity.Property(e => e.ThoiGianDat)
                .HasColumnType("datetime")
                .HasColumnName("thoiGianDat");
            entity.Property(e => e.ThoiGianKetThuc)
                .HasColumnType("datetime")
                .HasColumnName("thoiGianKetThuc");
            entity.Property(e => e.TrangThaiDonHang)
                .HasMaxLength(50)
                .HasColumnName("trangThaiDonHang");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasMaxLength(50)
                .HasColumnName("trangThaiThanhToan");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__maKhachH__300424B4");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__HoaDon__maNhanVi__30F848ED");

            entity.HasOne(d => d.MaVoucherNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaVoucher)
                .HasConstraintName("FK__HoaDon__maVouche__31EC6D26");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__0CCB3D49D960668A");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKhachHang).HasColumnName("maKhachHang");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("diaChi");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.GioiTinh).HasColumnName("gioiTinh");
            entity.Property(e => e.NgaySinh).HasColumnName("ngaySinh");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .HasColumnName("soDienThoai");
            entity.Property(e => e.TenKhachHang)
                .HasMaxLength(50)
                .HasColumnName("tenKhachHang");
        });

        modelBuilder.Entity<MonAn>(entity =>
        {
            entity.HasKey(e => e.MaMon).HasName("PK__MonAn__27547BFA7EFB4EBB");

            entity.ToTable("MonAn");

            entity.Property(e => e.MaMon).HasColumnName("maMon");
            entity.Property(e => e.ChiTietFood).HasColumnName("chiTietFood");
            entity.Property(e => e.Gia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia");
            entity.Property(e => e.HinhAnh)
                .HasMaxLength(255)
                .HasColumnName("hinhAnh");
            entity.Property(e => e.LoaiSanPham)
                .HasMaxLength(50)
                .HasColumnName("loaiSanPham");
            entity.Property(e => e.NgayCapNhat).HasColumnName("ngayCapNhat");
            entity.Property(e => e.NgayTao).HasColumnName("ngayTao");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.TenMon)
                .HasMaxLength(50)
                .HasColumnName("tenMon");
            entity.Property(e => e.TrangThai).HasColumnName("trangThai");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__BDDEF20DF78D35B1");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNhanVien).HasColumnName("maNhanVien");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("diaChi");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.GioiTinh).HasColumnName("gioiTinh");
            entity.Property(e => e.NgaySinh).HasColumnName("ngaySinh");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .HasColumnName("soDienThoai");
            entity.Property(e => e.TenNhanVien)
                .HasMaxLength(50)
                .HasColumnName("tenNhanVien");
            entity.Property(e => e.Tuoi).HasColumnName("tuoi");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__taiKhoan__8FFF6A9D576AD94F");

            entity.ToTable("taiKhoan");

            entity.HasIndex(e => e.MaKhachHang, "UQ_KhachHang").IsUnique();

            entity.HasIndex(e => e.MaNhanVien, "UQ_NhanVien").IsUnique();

            entity.Property(e => e.MaTaiKhoan).HasColumnName("maTaiKhoan");
            entity.Property(e => e.LoaiTaiKhoan)
                .HasMaxLength(50)
                .HasColumnName("loaiTaiKhoan");
            entity.Property(e => e.MaKhachHang).HasColumnName("maKhachHang");
            entity.Property(e => e.MaNhanVien).HasColumnName("maNhanVien");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(50)
                .HasColumnName("matKhau");
            entity.Property(e => e.TenTk)
                .HasMaxLength(50)
                .HasColumnName("tenTK");

            entity.HasOne(d => d.MaKhachHangNavigation).WithOne(p => p.TaiKhoan)
                .HasForeignKey<TaiKhoan>(d => d.MaKhachHang)
                .HasConstraintName("FK__taiKhoan__maKhac__3B75D760");

            entity.HasOne(d => d.MaNhanVienNavigation).WithOne(p => p.TaiKhoan)
                .HasForeignKey<TaiKhoan>(d => d.MaNhanVien)
                .HasConstraintName("FK__taiKhoan__maNhan__3C69FB99");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.MaVoucher).HasName("PK__Voucher__E335C400381BF7D9");

            entity.ToTable("Voucher");

            entity.Property(e => e.MaVoucher).HasColumnName("maVoucher");
            entity.Property(e => e.NgayBatDau).HasColumnName("ngayBatDau");
            entity.Property(e => e.NgayKetThuc).HasColumnName("ngayKetThuc");
            entity.Property(e => e.SoLanSuDungHienTai).HasColumnName("soLanSuDungHienTai");
            entity.Property(e => e.SoLanSuDungToiDa).HasColumnName("soLanSuDungToiDa");
            entity.Property(e => e.SoTienGiam)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("soTienGiam");
            entity.Property(e => e.TrangThai).HasColumnName("trangThai");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
