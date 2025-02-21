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
    public class NhanViensController : Controller
    {
        private readonly DoAnStoreContext _context;

        public NhanViensController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: NhanViens
        public async Task<IActionResult> Index()
        {
            return View(await _context.NhanViens.ToListAsync());
        }

        // GET: NhanViens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // GET: NhanViens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNhanVien,TenNhanVien,GioiTinh,Cccd,SoDienThoai,NgaySinh,DiaChi,Email,Tuoi,Avatar")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }

        // GET: NhanViens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }

        // POST: NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNhanVien,TenNhanVien,GioiTinh,Cccd,SoDienThoai,NgaySinh,DiaChi,Email,Tuoi,Avatar")] NhanVien nhanVien, IFormFile AvatarFile)
        {
            if (id != nhanVien.MaNhanVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingNhanVien = await _context.NhanViens.FindAsync(id);// Lấy thông tin khách hàng từ database
                    if (existingNhanVien == null)
                    {
                        return NotFound();
                    }
                    // Cập nhật thông tin khách hàng
                    existingNhanVien.TenNhanVien = nhanVien.TenNhanVien;
                    existingNhanVien.GioiTinh = nhanVien.GioiTinh;
                    existingNhanVien.SoDienThoai = nhanVien.SoDienThoai;
                    existingNhanVien.Cccd = nhanVien.Cccd;
                    existingNhanVien.NgaySinh = nhanVien.NgaySinh;
                    existingNhanVien.Email = nhanVien.Email;
                    existingNhanVien.DiaChi = nhanVien.DiaChi;
                    existingNhanVien.Tuoi = nhanVien.Tuoi;

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
                        existingNhanVien.Avatar = "/img/avatars/" + fileName;
                    }

                    _context.Update(existingNhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.MaNhanVien))
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
            return View(nhanVien);
        }

        // GET: NhanViens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(int id)
        {
            return _context.NhanViens.Any(e => e.MaNhanVien == id);
        }
    }
}
    