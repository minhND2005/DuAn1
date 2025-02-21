using Fast_Food.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fast_Food.Controllers
{
    public class DangNhapController : Controller
    {
        readonly DoAnStoreContext _context;

        // Constructor để khởi tạo _context thông qua Dependency Injection
        public DangNhapController(DoAnStoreContext context)
        {
            _context = context;
        }

        // Hiển thị trang đăng nhập
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string tk, string mk)
        {
            var taikhoan = _context.TaiKhoans.FirstOrDefault(t => t.TenTk == tk);

            if (taikhoan == null)
            {
                ViewBag.loi = "Sai tên đăng nhập!";
                return View();
            }

            if (!VerifyPassword(mk, taikhoan.MatKhau))
            {
                ViewBag.loi = "Sai mật khẩu!";
                return View();
            }

            string defaultAvatar = "/img/avatars/default-avatar.jpg"; // Ảnh mặc định nếu không có avatar

            // Kiểm tra loại tài khoản để lưu vào session
            if (taikhoan.LoaiTaiKhoan == "NhanVien")
            {
                if (taikhoan.MaNhanVien == null)
                {
                    ViewBag.loi = "Tài khoản nhân viên không hợp lệ!";
                    return View();
                }

                // Lưu thông tin vào session
                HttpContext.Session.SetString("MaNhanVien", taikhoan.MaNhanVien?.ToString() ?? "");
                HttpContext.Session.SetString("TenNhanVien", taikhoan.TenTk);
                HttpContext.Session.SetString("Avatar", taikhoan.MaNhanVienNavigation?.Avatar ?? defaultAvatar);

                return RedirectToAction("QuanLySanPham", "MonAns");
            }
            else if (taikhoan.LoaiTaiKhoan == "KhachHang")
            {
                if (taikhoan.MaKhachHang == null)
                {
                    ViewBag.loi = "Tài khoản khách hàng không hợp lệ!";
                    return View();
                }

                // Lưu thông tin vào session
                HttpContext.Session.SetString("MaKhachHang", taikhoan.MaKhachHang.ToString());
                HttpContext.Session.SetString("TenKhachHang", taikhoan.TenTk);
                HttpContext.Session.SetString("Avatar", taikhoan.MaKhachHangNavigation?.Avatar ?? defaultAvatar);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.loi = "Loại tài khoản không hợp lệ!";
            return View();
        }
        private bool VerifyPassword(string passwordInput, string passwordStored)
        {
            return passwordInput == passwordStored;
        }

        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "DangNhap");
        }

        // Hiển thị form đăng ký
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        // Xử lý logic đăng ký
        [HttpPost]
        public IActionResult DangKy(string Email, string Ten, string SoDienThoai, string DiaChi, string Tk, string MK1, string MK2, string GioiTinh, DateTime NgaySinh)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Ten) || string.IsNullOrWhiteSpace(Tk) ||
                string.IsNullOrWhiteSpace(MK1) || string.IsNullOrWhiteSpace(MK2))
            {
                ViewBag.loii = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            if (MK1 != MK2)
            {
                ViewBag.loii = "Mật khẩu không trùng khớp.";
                return View();
            }

            var existingAccount = _context.TaiKhoans.FirstOrDefault(t => t.TenTk == Tk);
            if (existingAccount != null)
            {
                ViewBag.loii = "Tài khoản đã tồn tại.";
                return View();
            }

            // Tạo mới khách hàng
            KhachHang khmoi = new KhachHang
            {
                TenKhachHang = Ten,
                Email = Email,
                SoDienThoai = SoDienThoai,
                DiaChi = DiaChi,
                GioiTinh = GioiTinh == "Nam",
                NgaySinh = DateOnly.FromDateTime(NgaySinh),
                Avatar = "default.jpg"
            };

            _context.KhachHangs.Add(khmoi);
            _context.SaveChanges();

            // Tạo tài khoản đăng nhập
            TaiKhoan tkmoi = new TaiKhoan
            {
                TenTk = Tk,
                MatKhau = MK1,
                LoaiTaiKhoan = "KhachHang",
                MaKhachHang = khmoi.MaKhachHang
            };

            _context.TaiKhoans.Add(tkmoi);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // Trang liên hệ
        public IActionResult LienHe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LienHe(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Message"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm.";
                return RedirectToAction("LienHe");
            }

            return View(model);
        }
    }
}
