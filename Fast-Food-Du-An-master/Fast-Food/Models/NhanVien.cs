using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class NhanVien
{
    public int MaNhanVien { get; set; }

    public string TenNhanVien { get; set; } = null!;

    public bool? GioiTinh { get; set; }

    public string? Cccd { get; set; }

    public string SoDienThoai { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public int? Tuoi { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual TaiKhoan? TaiKhoan { get; set; }
}
