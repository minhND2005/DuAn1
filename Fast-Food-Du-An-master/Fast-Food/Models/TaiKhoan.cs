using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class TaiKhoan 
{
    public int MaTaiKhoan { get; set; }

    public string TenTk { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int? MaKhachHang { get; set; }

    public int? MaNhanVien { get; set; }

    public string? LoaiTaiKhoan { get; set; }

    public virtual KhachHang? MaKhachHangNavigation { get; set; }

    public virtual NhanVien? MaNhanVienNavigation { get; set; }
}
