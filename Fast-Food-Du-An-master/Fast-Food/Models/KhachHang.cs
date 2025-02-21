using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class KhachHang
{
    public int MaKhachHang { get; set; }

    public string TenKhachHang { get; set; } = null!;

    public bool? GioiTinh { get; set; }

    public string SoDienThoai { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual TaiKhoan? TaiKhoan { get; set; }
}
