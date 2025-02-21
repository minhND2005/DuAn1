using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class MonAn
{
    public int MaMon { get; set; }

    public string? LoaiSanPham { get; set; }

    public string TenMon { get; set; } = null!;

    public decimal Gia { get; set; }

    public int SoLuong { get; set; }

    public bool? TrangThai { get; set; }

    public DateOnly? NgayTao { get; set; }

    public DateOnly? NgayCapNhat { get; set; }

    public string? ChiTietFood { get; set; }

    public string? HinhAnh { get; set; }

    public virtual ICollection<ChiTietCombo> ChiTietCombos { get; set; } = new List<ChiTietCombo>();

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual ICollection<GioHang> GioHangMaComboNavigations { get; set; } = new List<GioHang>();

    public virtual ICollection<GioHang> GioHangMaMonNavigations { get; set; } = new List<GioHang>();
}
