using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class Voucher
{
    public int MaVoucher { get; set; }

    public decimal SoTienGiam { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public bool? TrangThai { get; set; }

    public int SoLanSuDungToiDa { get; set; }

    public int SoLanSuDungHienTai { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
