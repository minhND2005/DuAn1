using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fast_Food.Models;

namespace Fast_Food.Controllers
{
    public class HoaDonsController : Controller
    {
        private readonly DoAnStoreContext _context;

        public HoaDonsController(DoAnStoreContext context)
        {
            _context = context;
        }
       

        [HttpPost]
        public IActionResult ThanhToan(int id)
        {
            var hoaDon = _context.HoaDons.Find(id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái đơn hàng thành "Đã thanh toán"
            hoaDon.TrangThaiThanhToan = "Đã thanh toán";
            _context.Update(hoaDon);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được thanh toán thành công!";

            // Chuyển hướng đến trang danh sách hóa đơn
            return RedirectToAction("Index");
        }
        // GET: HoaDons
        public async Task<IActionResult> Index()
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maKhachHang))
            {
                return RedirectToAction("Login", "DangNhap"); // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            }

            int maKH = int.Parse(maKhachHang); // Chuyển đổi mã khách hàng từ string sang int

            var hoaDons = await _context.HoaDons
                .Where(h => h.MaKhachHang == maKH) // Lọc theo mã khách hàng
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.MaNhanVienNavigation)
                .Include(h => h.MaVoucherNavigation)
                .ToListAsync();

            return View(hoaDons);
        }
        public IActionResult CancelOrder(int id)
        {
            var hoaDon = _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (hoaDon == null || hoaDon.TrangThaiDonHang == "Đã hủy")
            {
                return NotFound();
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Cập nhật trạng thái hóa đơn
                    hoaDon.TrangThaiDonHang = "Đã hủy";
                    hoaDon.TrangThaiThanhToan = "Đã hủy";
                    hoaDon.ThoiGianKetThuc = DateTime.Now;
                    // Hoàn lại số lượng món ăn
                    foreach (var chiTiet in hoaDon.ChiTietHoaDons)
                    {
                        var monAn = _context.MonAns.Find(chiTiet.MaMon);
                        if (monAn != null)
                        {
                            monAn.SoLuong += chiTiet.SoLuong.Value;
                            monAn.TrangThai = true; // Món ăn có hàng trở lại
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> HoaDon(string status)
        {
            // Kiểm tra session người dùng đã đăng nhập hay chưa
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");

            // Nếu chưa đăng nhập, chuyển hướng về trang Login
            if (string.IsNullOrEmpty(maKhachHang) && string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "DangNhap");
            }

            var hoaDons = _context.HoaDons
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.MaNhanVienNavigation)
                .Include(h => h.MaVoucherNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "Canceled":
                        hoaDons = hoaDons.Where(h => h.TrangThaiDonHang == "Đã hủy");
                        break;
                    case "Preparing":
                        hoaDons = hoaDons.Where(h => h.TrangThaiDonHang == "Đang chuẩn bị");
                        break;
                    case "Shipping":
                        hoaDons = hoaDons.Where(h => h.TrangThaiDonHang == "Đang giao hàng");
                        break;
                    case "DaThanhToan":
                        hoaDons = hoaDons.Where(h => h.TrangThaiDonHang == "Đã thanh toán");
                        break;
                    case "HoanThanh":
                        hoaDons = hoaDons.Where(h => h.TrangThaiDonHang == "Hoàn thành");
                        break;
                    default:
                        break;
                }
            }

            var hoaDonsList = await hoaDons.ToListAsync();
            return View(hoaDonsList);
        }


        // GET: Hiển thị form chỉnh sửa trạng thái đơn hàng
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("HoaDon");
            }

            string trangThaiHienTai = hoaDon.TrangThaiDonHang.Trim().ToLower();

            switch (trangThaiHienTai)
            {
                case "đang chuẩn bị":
                    hoaDon.TrangThaiDonHang = "Đang giao hàng";
                    hoaDon.TrangThaiThanhToan = "Đang xử lý";
                    break;
                case "đang giao hàng":
                    hoaDon.TrangThaiDonHang = "Đã thanh toán";
                    hoaDon.TrangThaiThanhToan = "Đã thanh toán";
                    break;
                case "đã thanh toán":
                    hoaDon.TrangThaiDonHang = "Hoàn thành";
                    hoaDon.TrangThaiThanhToan = "Đã thanh toán";
                    hoaDon.ThoiGianKetThuc = DateTime.Now;
                    break;
                case "hoàn thành":
                    TempData["ErrorMessage"] = "Đơn hàng đã hoàn thành, không thể cập nhật tiếp!";
                    return RedirectToAction("HoaDon");
                default:
                    TempData["ErrorMessage"] = "Trạng thái đơn hàng không hợp lệ!";
                    return RedirectToAction("HoaDon");
            }

            _context.HoaDons.Update(hoaDon);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("HoaDon");
        }


        // GET: HoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.MaNhanVienNavigation)
                .Include(h => h.MaVoucherNavigation)
                .Include(h => h.ChiTietHoaDons) // Lấy chi tiết hóa đơn
                    .ThenInclude(ct => ct.MaMonNavigation) // Lấy thông tin món ăn
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }
        // GET: HoaDons/Create
        public IActionResult Create()
        {
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang");
            ViewData["MaNhanVien"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien");
            ViewData["MaVoucher"] = new SelectList(_context.Vouchers, "MaVoucher", "MaVoucher");
            return View();
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHoaDon,MaKhachHang,MaNhanVien,ThoiGianDat,ThoiGianKetThuc,TrangThaiDonHang,TrangThaiThanhToan,SdtlienHe,DiaChiGiaoHang,DanhGia,MaVoucher")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", hoaDon.MaKhachHang);
            ViewData["MaNhanVien"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", hoaDon.MaNhanVien);
            ViewData["MaVoucher"] = new SelectList(_context.Vouchers, "MaVoucher", "MaVoucher", hoaDon.MaVoucher);
            return View(hoaDon);
        }

        // GET: HoaDons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", hoaDon.MaKhachHang);
            ViewData["MaNhanVien"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", hoaDon.MaNhanVien);
            ViewData["MaVoucher"] = new SelectList(_context.Vouchers, "MaVoucher", "MaVoucher", hoaDon.MaVoucher);
            return View(hoaDon);
        }

        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHoaDon,MaKhachHang,MaNhanVien,ThoiGianDat,ThoiGianKetThuc,TrangThaiDonHang,TrangThaiThanhToan,SdtlienHe,DiaChiGiaoHang,DanhGia,MaVoucher")] HoaDon hoaDon)
        {
            if (id != hoaDon.MaHoaDon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHoaDon))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", hoaDon.MaKhachHang);
            ViewData["MaNhanVien"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", hoaDon.MaNhanVien);
            ViewData["MaVoucher"] = new SelectList(_context.Vouchers, "MaVoucher", "MaVoucher", hoaDon.MaVoucher);
            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.MaNhanVienNavigation)
                .Include(h => h.MaVoucherNavigation)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ThongKe()
        {
            var thongKe = _context.HoaDons
                .GroupBy(hd => hd.TrangThaiDonHang)
                .Select(g => new ThongKeViewModel
                {
                    TrangThai = g.Key,
                    TongTien = g.Sum(hd => hd.TongTien) ?? 0 // Tránh lỗi null
                })
                .ToList();

            return View(thongKe); // Trả về danh sách ThongKeViewModel
        }
        public async Task<IActionResult> XacNhanDon()
        {
            var hoaDons = await _context.HoaDons
                .Where(h => h.TrangThaiDonHang == "Chờ xác nhận") // Chỉ lấy đơn hàng chưa xác nhận
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.MaNhanVienNavigation)
                .Include(h => h.MaVoucherNavigation)
                .ToListAsync();

            if (hoaDons == null || !hoaDons.Any())
            {
                ViewBag.Message = "Không có hóa đơn nào cần xác nhận.";
            }

            return View(hoaDons);
        }

        [HttpPost]
        public async Task<IActionResult> XacNhan(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null && hoaDon.TrangThaiDonHang == "Chờ xác nhận") 
            {
                hoaDon.TrangThaiDonHang = "Đang chuẩn bị"; 
                await _context.SaveChangesAsync(); 
            }
            return RedirectToAction("XacNhanDon");
        }


        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.MaHoaDon == id);
        }
        [HttpPost]
        public IActionResult HuyDonHang(int id)
        {
            var MaNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(MaNhanVien))
            {
                return RedirectToAction("Login", "DangNhap");
            }

            var hoaDon = _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (hoaDon == null || hoaDon.TrangThaiDonHang == "Đã hủy")
            {
                TempData["ErrorMessage"] = "Đơn hàng không hợp lệ hoặc đã bị hủy.";
                return RedirectToAction("Index");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Cập nhật trạng thái hóa đơn
                    hoaDon.TrangThaiDonHang = "Đã hủy";
                    hoaDon.TrangThaiThanhToan = "Đã hủy";
                    hoaDon.ThoiGianKetThuc = DateTime.Now;

                    // Hoàn lại số lượng món ăn
                    foreach (var chiTiet in hoaDon.ChiTietHoaDons)
                    {
                        var monAn = _context.MonAns.Find(chiTiet.MaMon);
                        if (monAn != null)
                        {
                            monAn.SoLuong += chiTiet.SoLuong.Value;
                            monAn.TrangThai = true;
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công!";
                }
                catch
                {
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy đơn hàng.";
                }
            }

            return RedirectToAction("XacNhanDon");
        }
      
    }
}
