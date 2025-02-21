using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class ChiTietHoaDon
{
    public int MaChiTietHoaDon { get; set; }

    public int? MaMon { get; set; }

    public int MaHoaDon { get; set; }

    public string? TenMonAn { get; set; }

    public int? SoLuong { get; set; }

    public decimal? Gia { get; set; }

    public decimal? TongTien { get; set; }

    public string? GhiChu { get; set; }

    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;

    public virtual MonAn? MaMonNavigation { get; set; }
}
