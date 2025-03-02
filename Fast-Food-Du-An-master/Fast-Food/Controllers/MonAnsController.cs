    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Fast_Food.Models;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Claims;

namespace Fast_Food.Controllers
{
    public class MonAnsController : Controller
    {
        private readonly DoAnStoreContext _context;

        public MonAnsController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: MonAns
        public async Task<IActionResult> Index(string LoaiSanPham, string TimKiem, int pg = 1)
        {
            const int pageSize = 12; // Số sản phẩm trên mỗi trang

            // Truy vấn dữ liệu từ database
            IQueryable<MonAn> monAnQuery = _context.MonAns;

            // Lọc theo từ khóa tìm kiếm (tìm trong tên món ăn)
            if (!string.IsNullOrEmpty(TimKiem))
            {
                monAnQuery = monAnQuery.Where(m => m.TenMon.ToLower().Contains(TimKiem.ToLower()));
            }

            // Tổng số bản ghi sau khi lọc
            int recsCount = await monAnQuery.CountAsync();

            // Tính toán phân trang
            var pager = new demdanhsach(recsCount, pg, pageSize);
                ViewBag.Pager = pager;
                ViewBag.LoaiSanPham = LoaiSanPham;
                ViewBag.TimKiem = TimKiem;

            // Lấy dữ liệu cho trang hiện tại
            var products = await monAnQuery
                .Skip((pg - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(products);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, string soluong)
        {
            // Lấy MaKhachHang từ Session
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");
            if (maKhachHang == null)
            {
                return RedirectToAction("Login", "DangNhap");
            }

            int maKH = int.Parse(maKhachHang);

            // Chuyển đổi soluong từ string -> int
            if (!int.TryParse(soluong, out int soLuong) || soLuong <= 0)
            {
                return BadRequest("Số lượng phải là số nguyên dương.");
            }

            // Kiểm tra món ăn có tồn tại không
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            // Kiểm tra số lượng tồn kho
            if (monAn.SoLuong < soLuong)
            {
                TempData["Error1"] = $"Món ăn {monAn.TenMon} chỉ còn lại {monAn.SoLuong} trong cửa hàng.";
                return RedirectToAction("Details", new { id = id });
            }

            // Kiểm tra xem món ăn đã có trong giỏ hàng chưa
            var giohang = await _context.GioHangs
                .FirstOrDefaultAsync(m => m.MaMon == id && m.MaKhachHang == maKH);

            if (giohang != null)
            {
                // Nếu đã có, cộng dồn số lượng
                giohang.SoLuong += soLuong;

                // Kiểm tra lại số lượng tồn kho trước khi lưu
                if (monAn.SoLuong < giohang.SoLuong)
                {
                    TempData["Error1"] = $"Món ăn {monAn.TenMon} chỉ còn lại {monAn.SoLuong} trong cửa hàng.";
                    return RedirectToAction("Details", new { id = id });
                }

                // Cập nhật lại giá tiền theo số lượng mới
                giohang.Gia = monAn.Gia * giohang.SoLuong;

                _context.GioHangs.Update(giohang);
            }
            else
            {
                // Nếu chưa có, thêm mới
                giohang = new GioHang
                {
                    MaKhachHang = maKH,
                    MaMon = monAn.MaMon,
                    TenSanPham = monAn.TenMon,
                    SoLuong = soLuong,
                    Gia = monAn.Gia * soLuong, 
                    anh = monAn.HinhAnh,
                    GhiChu = monAn.TenMon
                };

                _context.GioHangs.Add(giohang);
            }

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

       



        [HttpPost]
        public async Task<IActionResult> BuyNow(int id, int soLuong)
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            if (string.IsNullOrEmpty(maKhachHang))
            {
                return RedirectToAction("Login", "DangNhap"); // Chuyển hướng nếu chưa đăng nhập
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.MaKhachHang == int.Parse(maKhachHang));

            if (khachHang == null)
            {
                return BadRequest("Không tìm thấy thông tin khách hàng.");
            }

            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null || monAn.SoLuong < soLuong)
            {
                return BadRequest("Sản phẩm không đủ hàng.");
            }

            // Tạo hóa đơn mới
            var hoaDon = new HoaDon
            {
                MaKhachHang = khachHang.MaKhachHang,
                ThoiGianDat = DateTime.Now,
                TrangThaiDonHang = "Chờ xác nhận",
                TrangThaiThanhToan = "Đang xử lý",
                SdtlienHe = khachHang.SoDienThoai,
                DiaChiGiaoHang = khachHang.DiaChi,
                TongTien = monAn.Gia * soLuong,
                DanhGia = 0,
                ChiTietHoaDons = new List<ChiTietHoaDon>()
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            // Thêm sản phẩm vào chi tiết hóa đơn với số lượng từ form
            var chiTietHoaDon = new ChiTietHoaDon
            {
                MaHoaDon = hoaDon.MaHoaDon,
                MaMon = id,
                SoLuong = soLuong,  //  Cập nhật số lượng từ form
                Gia = monAn.Gia * soLuong
            };

            _context.ChiTietHoaDons.Add(chiTietHoaDon);

            // Cập nhật số lượng sản phẩm
            monAn.SoLuong -= soLuong;
            if (monAn.SoLuong == 0)
            {
                monAn.TrangThai = false; // Cập nhật trạng thái hết hàng
            }

            _context.MonAns.Update(monAn);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "HoaDons", new { id = hoaDon.MaHoaDon });
        }

        // GET: MonAns/Details/5
        public async Task<IActionResult> Details(int? id, string? loaiSanPham)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monAn = await _context.MonAns
                .FirstOrDefaultAsync(m => m.MaMon == id);
            if (monAn == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(loaiSanPham))
            {
                loaiSanPham = monAn.LoaiSanPham;
            }

            ViewData["LoaiSanPham"] = loaiSanPham;
            return View(monAn);
        }

        // GET: MonAns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MonAns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: MonAns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaMon,LoaiSanPham,TenMon,Gia,SoLuong,TrangThai,NgayTao,NgayCapNhat,ChiTietFood")] MonAn monAn, IFormFile HinhAnh)
        {
            // Kiểm tra nếu monAn là null
            if (monAn == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            // Kiểm tra nếu LoaiSanPham là "0" (Lựa Chọn Món Ăn)
            if (monAn.LoaiSanPham == "0")
            {
                ModelState.AddModelError("LoaiSanPham", "Vui lòng chọn loại món ăn hợp lệ.");
                ViewBag.LoaiSanPhamList = new List<SelectListItem>
             {
                 new SelectListItem { Value = "0", Text = "---Lựa Chọn Món Ăn---" },
                 new SelectListItem { Value = "1", Text = "Gà" },
                 new SelectListItem { Value = "2", Text = "Kem" },
                 new SelectListItem { Value = "3", Text = "Burger" },
                 new SelectListItem { Value = "4", Text = "Pizza" },
                 new SelectListItem { Value = "5", Text = "Khoai" },
                 new SelectListItem { Value = "6", Text = "Đồ Uống" },
                 new SelectListItem { Value = "7", Text = "Combo" }
             };
                return View(monAn);
            }

            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                ViewBag.LoaiSanPhamList = new List<SelectListItem>
             {
                 new SelectListItem { Value = "0", Text = "---Lựa Chọn Món Ăn---" },
                 new SelectListItem { Value = "1", Text = "Gà" },
                 new SelectListItem { Value = "2", Text = "Kem" },
                 new SelectListItem { Value = "3", Text = "Burger" },
                 new SelectListItem { Value = "4", Text = "Pizza" },
                 new SelectListItem { Value = "5", Text = "Khoai" },
                 new SelectListItem { Value = "6", Text = "Đồ Uống" },
                 new SelectListItem { Value = "7", Text = "Combo" }
             };
                return View(monAn);
            }

            // Xử lý upload ảnh
            if (HinhAnh != null && HinhAnh.Length > 0)// Kiểm tra file có rỗng không
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/monan");// Đường dẫn thư mục chứa ảnh
                if (!Directory.Exists(folderPath))// Kiểm tra thư mục chứa ảnh có tồn tại không
                {
                    Directory.CreateDirectory(folderPath);// Tạo thư mục nếu chưa tồn tại
                }

                var fileExtension = Path.GetExtension(HinhAnh.FileName);// Lấy đuôi file
                var fileName = $"{Guid.NewGuid()}{fileExtension}";// Tạo tên file mới
                var filePath = Path.Combine(folderPath, fileName);// Tạo đường dẫn file

                using (var stream = new FileStream(filePath, FileMode.Create))// Tạo file mới
                {
                    await HinhAnh.CopyToAsync(stream);// Copy file
                }

                // Sử dụng Path.Combine nhưng thay bằng cách nối chuỗi thủ công
                monAn.HinhAnh = "img/monan/" + fileName;



            }

            monAn.NgayTao = DateOnly.FromDateTime(DateTime.Now);// Lưu ngày tạo
            monAn.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now);// Lưu ngày cập nhật

            _context.Add(monAn);// Thêm món ăn vào database
            await _context.SaveChangesAsync();// Lưu thay đổi

            return RedirectToAction(nameof(Index));// Chuyển hướng về trang Index
        }
        // GET: MonAns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null)
            {
                return NotFound();
            }
            return View(monAn);
        }

        // POST: MonAns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaMon,LoaiSanPham,TenMon,Gia,SoLuong,TrangThai,NgayTao,NgayCapNhat,ChiTietFood,HinhAnh")] MonAn monAn, IFormFile file)
        {
            if (id != monAn.MaMon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy món ăn cũ từ DB
                    var monAnCu = await _context.MonAns.FirstOrDefaultAsync(m => m.MaMon == id);
                    if (monAnCu == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin cơ bản
                    monAnCu.TenMon = monAn.TenMon;
                    monAnCu.SoLuong = monAn.SoLuong;
                    monAnCu.Gia = monAn.Gia;
                    monAnCu.ChiTietFood = monAn.ChiTietFood;
                    monAnCu.TrangThai = monAn.TrangThai;
                    monAnCu.LoaiSanPham = monAn.LoaiSanPham;
                    monAnCu.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now);

                    // Nếu có upload file mới
                    if (file != null && file.Length > 0)
                    {
                        // Tạo tên file mới để tránh trùng (thêm timestamp)
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        string newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                        // Định nghĩa đường dẫn lưu file
                        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/monan");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath); // Tạo thư mục nếu chưa tồn tại
                        }
                        string filePath = Path.Combine(folderPath, newFileName);

                        // Lưu file vào thư mục wwwroot/img/monan
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Cập nhật lại đường dẫn ảnh trong database
                        monAnCu.HinhAnh = Path.Combine("img/monan/", newFileName);

                    }
                    else
                    {
                        // Nếu không upload ảnh mới, giữ nguyên ảnh cũ
                        monAn.HinhAnh = monAnCu.HinhAnh;
                    }
                    _context.Update(monAnCu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonAnExists(monAn.MaMon))
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
            return View(monAn);
        }



        // GET: MonAns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monAn = await _context.MonAns
                .FirstOrDefaultAsync(m => m.MaMon == id);
            if (monAn == null)
            {
                return NotFound();
            }

            return View(monAn);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Tìm tất cả các bản ghi liên quan trong chiTietHoaDon
            var chiTietHoaDonList = _context.ChiTietHoaDons.Where(c => c.MaMon == id).ToList();

            // Xóa tất cả các bản ghi trong chiTietHoaDon trước
            if (chiTietHoaDonList.Any())
            {
                _context.ChiTietHoaDons.RemoveRange(chiTietHoaDonList);
            }

            // Xóa MonAn sau khi đã xóa chi tiết hóa đơn
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn != null)
            {
                _context.MonAns.Remove(monAn);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("QuanLySanPham");
        }

        public async Task<IActionResult> QuanLySanPham(string TimKiem)
        {
            var monAns = _context.MonAns.AsQueryable();

            // Nếu có tham số TimKiem, lọc theo Tên Món
            if (!string.IsNullOrEmpty(TimKiem))
            {
                monAns = monAns.Where(m => m.TenMon.Contains(TimKiem));
            }

            return View(monAns.ToList());
        }


        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(int id, int soLuong)
        {
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null)
            {
                return NotFound();
            }

            if (soLuong < 0)
            {
                TempData["ErrorMessage"] = "Số lượng không thể âm!";
                return RedirectToAction(nameof(QuanLySanPham));
            }

            monAn.SoLuong = soLuong;

            // Cập nhật trạng thái còn hàng hoặc hết hàng
            monAn.TrangThai = soLuong > 0;

            _context.Update(monAn);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật số lượng thành công!";
            return RedirectToAction(nameof(QuanLySanPham));
        }
        private bool MonAnExists(int id)
        {
            return _context.MonAns.Any(e => e.MaMon == id);
        }
    }
}
