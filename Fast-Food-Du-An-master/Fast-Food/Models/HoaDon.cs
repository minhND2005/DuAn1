using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class HoaDon
{
    public int MaHoaDon { get; set; }

    public int MaKhachHang { get; set; }

    public int? MaNhanVien { get; set; }

    public DateTime ThoiGianDat { get; set; }

    public DateTime? ThoiGianKetThuc { get; set; }

    public string? TrangThaiDonHang { get; set; }

    public string? TrangThaiThanhToan { get; set; }

    public string? SdtlienHe { get; set; }

    public string? DiaChiGiaoHang { get; set; }

    public double DanhGia { get; set; }

    public int? MaVoucher { get; set; }
    public decimal? TongTien { get; set; } 

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual NhanVien? MaNhanVienNavigation { get; set; }

    public virtual Voucher? MaVoucherNavigation { get; set; }
}
