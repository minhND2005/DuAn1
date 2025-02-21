using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class GioHang
{
    public int MaGioHang { get; set; }

    public int MaKhachHang { get; set; }

    public int? MaMon { get; set; }

    public int? MaCombo { get; set; }

    public string? TenSanPham { get; set; }

    public decimal? Gia { get; set; }

    public int? SoLuong { get; set; }

    public string? GhiChu { get; set; }
    public string? anh { get; set; }
    public virtual MonAn? MaComboNavigation { get; set; }

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual MonAn? MaMonNavigation { get; set; }
}
