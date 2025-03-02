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
    public class GioHangsController : Controller
    {
        private readonly DoAnStoreContext _context;

        public GioHangsController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: GioHangs
        public async Task<IActionResult> Index()
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            if (maKhachHang == null)
            {
                return RedirectToAction("Login", "DangNhap");
            }

            int maKH = int.Parse(maKhachHang);

            // Sử dụng Include để lấy thông tin sản phẩm liên quan
            var gioHangs = await _context.GioHangs
                .Include(g => g.MaMonNavigation)
                .Where(g => g.MaKhachHang == maKH)
                .ToListAsync();

			var totalPrice = gioHangs.Sum(x => x.Gia);
			ViewData["TotalPrice"] = totalPrice;

			return View(gioHangs);
        }


        public async Task<IActionResult> MuaSanPham()
        {
            // Lấy mã khách hàng từ session
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHang))
            {
                return RedirectToAction("Login", "DangNhap"); // Nếu chưa đăng nhập, chuyển hướng tới trang login
            }

            int maKH = int.Parse(maKhachHang);

            // Lấy giỏ hàng của khách hàng
            var gioHang = await _context.GioHangs
                .Include(g => g.MaKhachHangNavigation)
                .Include(g => g.MaMonNavigation)
                .Where(g => g.MaKhachHang == maKH)
                .ToListAsync();

            if (!gioHang.Any()) // Nếu giỏ hàng trống, thông báo lỗi
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn không có sản phẩm!";
                return RedirectToAction("Index", "GioHangs");
            }

            // Tính tổng tiền chính xác và chỉ làm tròn khi cần thiết
            // Tính tổng tiền chỉ dựa trên giá của từng món
            var tongTien = gioHang.Sum(g => g.Gia);

            tongTien = (decimal)Math.Round((double)tongTien, 0);

            // Lấy thông tin khách hàng từ giỏ hàng (đã Include ở trên)
            var khachHang = gioHang.First().MaKhachHangNavigation;

            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("Index", "GioHangs");
            }

            // Tạo hóa đơn mới
            var hoaDon = new HoaDon
            {
                MaKhachHang = maKH,
                ThoiGianDat = DateTime.Now,
                TrangThaiDonHang = "Chờ xác nhận",
                TongTien = tongTien,
                SdtlienHe = khachHang.SoDienThoai,
                DiaChiGiaoHang = khachHang.DiaChi,
                TrangThaiThanhToan = "Đang xử lý"
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            // Lấy ID hóa đơn vừa tạo
            int maHoaDon = hoaDon.MaHoaDon;

            // Chuyển dữ liệu từ giỏ hàng thành chi tiết hóa đơn
            var chiTietHoaDons = gioHang.Select(item => new ChiTietHoaDon
            {
                MaHoaDon = maHoaDon,
                MaMon = item.MaMon,
                TenMonAn = item.MaMonNavigation?.TenMon, // Lấy tên món ăn đã Include sẵn
                SoLuong = item.SoLuong,
                Gia = item.Gia
            }).ToList();

            _context.ChiTietHoaDons.AddRange(chiTietHoaDons);

            // Trừ số lượng tồn sau khi thanh toán
            foreach (var item in gioHang)
            {
                // Tìm món ăn tương ứng
                var monAn = await _context.MonAns.FirstOrDefaultAsync(m => m.MaMon == item.MaMon);

                if (monAn != null)
                {
                    // Kiểm tra tồn kho trước khi trừ
                    if (monAn.SoLuong < item.SoLuong)
                    {
                        TempData["ErrorMessage"] = $"Sản phẩm {monAn.TenMon} không đủ số lượng tồn!";
                        return RedirectToAction("Index", "GioHangs");
                    }

                    // Trừ số lượng tồn
                    // Đảm bảo SoLuong không phải null trước khi trừ
                    monAn.SoLuong -= (int)item.SoLuong;

                    // Nếu muốn kiểm tra số lượng tồn không âm
                    if (monAn.SoLuong < 0)
                    {
                        monAn.SoLuong = 0;
                    }

                    _context.MonAns.Update(monAn);
                }
            }

            // Lưu thay đổi số lượng tồn
            await _context.SaveChangesAsync();

            // Xóa sản phẩm khỏi giỏ hàng sau khi mua
            _context.GioHangs.RemoveRange(gioHang);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mua hàng thành công!";
            return RedirectToAction("Index", "HoaDons");
        }

        // GET: GioHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.MaComboNavigation)
                .Include(g => g.MaKhachHangNavigation)
                .Include(g => g.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaGioHang == id);
            if (gioHang == null)
            {
                return NotFound();
            }

            return View(gioHang);
        }

        // GET: GioHangs/Create
        public IActionResult Create()
        {
            ViewData["MaCombo"] = new SelectList(_context.MonAns, "MaMon", "MaMon");
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang");
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon");
            return View();
        }

        // POST: GioHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGioHang,MaKhachHang,MaMon,MaCombo,TenSanPham,Gia,SoLuong,GhiChu")] GioHang gioHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gioHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaCombo"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaCombo);
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", gioHang.MaKhachHang);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaMon);
            return View(gioHang);
        }

        // GET: GioHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHang = await _context.GioHangs.FindAsync(id);
            if (gioHang == null)
            {
                return NotFound();
            }
            ViewData["MaCombo"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaCombo);
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", gioHang.MaKhachHang);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaMon);
            return View(gioHang);
        }

        // POST: GioHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaGioHang,MaKhachHang,MaMon,MaCombo,TenSanPham,Gia,SoLuong,GhiChu")] GioHang gioHang)
        {
            if (id != gioHang.MaGioHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gioHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GioHangExists(gioHang.MaGioHang))
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
            ViewData["MaCombo"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaCombo);
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", gioHang.MaKhachHang);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaMon);
            return View(gioHang);
        }
        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(int id, int soLuong)
        {
            // Kiểm tra nếu số lượng không hợp lệ
            if (soLuong <= 0)
            {
                TempData["Error1"] = "Số lượng phải lớn hơn 0.";
                return RedirectToAction("Index");
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.MaMonNavigation)
                .FirstOrDefaultAsync(g => g.MaGioHang == id);

            if (gioHang == null)
            {
                TempData["Error1"] = "Sản phẩm không tồn tại trong giỏ hàng.";
                return RedirectToAction("Index");
            }

            var monAn = gioHang.MaMonNavigation;

            // Kiểm tra tồn kho trước khi cập nhật
            if (monAn.SoLuong < soLuong)
            {
                TempData["Error2"] = $"Sản phẩm {monAn.TenMon} chỉ còn lại {monAn.SoLuong} trong kho.";
                return RedirectToAction("Index");
            }

            // Cập nhật số lượng và giá tiền
            gioHang.SoLuong = soLuong;
            gioHang.Gia = monAn.Gia * soLuong;

            _context.GioHangs.Update(gioHang);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var gioHang = await _context.GioHangs.FindAsync(id);
            if (gioHang != null)
            {
                _context.GioHangs.Remove(gioHang);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index"); // Quay lại trang giỏ hàng
        }

        private bool GioHangExists(int id)
        {
            return _context.GioHangs.Any(e => e.MaGioHang == id);
        }

        public IActionResult EditKhachHang()
        {
            // Lấy MaKhachHang từ Session
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            // Kiểm tra nếu không có MaKhachHang thì quay lại trang Login
            if (maKhachHang == null)
            {
                return RedirectToAction("Login", "DangNhap");
            }

            // Chuyển MaKhachHang từ string sang int
            int maKhachHangInt = int.Parse(maKhachHang);

            // Tìm khách hàng theo MaKhachHang
            var khachHang = _context.KhachHangs
                                    .FirstOrDefault(kh => kh.MaKhachHang == maKhachHangInt);

            // Nếu không tìm thấy, quay về Index
            if (khachHang == null)
            {
                TempData["Error2"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("Index");
            }

            return View(khachHang);
        }

        [HttpPost]
        public IActionResult EditKhachHang(KhachHang khachHang)
        {
            // Lấy MaKhachHang từ Session
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            if (maKhachHang == null)
            {
                return RedirectToAction("Login", "DangNhap");
            }

            // Chuyển MaKhachHang từ string sang int
            int maKhachHangInt = int.Parse(maKhachHang);

            // Tìm KhachHang theo MaKhachHang từ Session
            var khachHangDb = _context.KhachHangs
                                     .FirstOrDefault(kh => kh.MaKhachHang == maKhachHangInt);

            // Nếu tìm thấy, cập nhật thông tin
            if (khachHangDb != null)
            {
                khachHangDb.TenKhachHang = khachHang.TenKhachHang;
                khachHangDb.SoDienThoai = khachHang.SoDienThoai;
                khachHangDb.DiaChi = khachHang.DiaChi;

                _context.KhachHangs.Update(khachHangDb);
                _context.SaveChanges();

                TempData["Success"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                TempData["Error2"] = "Không tìm thấy thông tin khách hàng!";
            }

            return RedirectToAction("Index");
        }

    }
}
