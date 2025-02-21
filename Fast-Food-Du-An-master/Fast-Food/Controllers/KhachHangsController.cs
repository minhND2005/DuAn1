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
    public class KhachHangsController : Controller
    {
        private readonly DoAnStoreContext _context;

        public KhachHangsController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: KhachHangs
        public async Task<IActionResult> Index()
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            if (string.IsNullOrEmpty(maKhachHang))
            {
                return RedirectToAction("Index", "Login");
            }

            if (!int.TryParse(maKhachHang, out int maKhachHangInt))
            {
                return BadRequest("Mã khách hàng không hợp lệ.");
            }

            // Lấy thông tin khách hàng từ database
            var thongTinKhachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.MaKhachHang == maKhachHangInt);

            if (thongTinKhachHang == null)
            {
                return NotFound("Không tìm thấy thông tin khách hàng.");
            }

            // Trả về view Edit với model đúng kiểu
            return RedirectToAction("Edit", new { id = maKhachHangInt });
        }

        // GET: KhachHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKhachHang == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // GET: KhachHangs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhachHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhachHang,TenKhachHang,GioiTinh,SoDienThoai,NgaySinh,Email,DiaChi,Avatar")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khachHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khachHang);
        }

        // GET: KhachHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKhachHang,TenKhachHang,GioiTinh,SoDienThoai,NgaySinh,Email,DiaChi")] KhachHang khachHang, IFormFile AvatarFile)
        {
            if (id != khachHang.MaKhachHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingKhachHang = await _context.KhachHangs.FindAsync(id);// Lấy thông tin khách hàng từ database
                    if (existingKhachHang == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin khách hàng
                    existingKhachHang.TenKhachHang = khachHang.TenKhachHang;
                    existingKhachHang.GioiTinh = khachHang.GioiTinh;
                    existingKhachHang.SoDienThoai = khachHang.SoDienThoai;
                    existingKhachHang.NgaySinh = khachHang.NgaySinh;
                    existingKhachHang.Email = khachHang.Email;
                    existingKhachHang.DiaChi = khachHang.DiaChi;

                    // Xử lý upload ảnh đại diện
                    if (AvatarFile != null && AvatarFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(AvatarFile.FileName);// Lấy tên file
                        var filePath = Path.Combine("wwwroot/img/avatars", fileName);// Đường dẫn lưu file

                        // Lưu ảnh vào thư mục server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await AvatarFile.CopyToAsync(stream);
                        }

                        // Cập nhật đường dẫn ảnh trong database
                        existingKhachHang.Avatar = "/img/avatars/" + fileName;
                    }

                    _context.Update(existingKhachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.KhachHangs.Any(e => e.MaKhachHang == khachHang.MaKhachHang)) // Kiểm tra khách hàng có tồn tại không
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
            return View(khachHang);
        }
        // GET: KhachHangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKhachHang == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // POST: KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                _context.KhachHangs.Remove(khachHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DanhSachKhachHang()
        {
            var khachHangs = _context.KhachHangs.ToList();
            return View(khachHangs);
        }

        private bool KhachHangExists(int id)
        {
            return _context.KhachHangs.Any(e => e.MaKhachHang == id);
        }
    }
}
