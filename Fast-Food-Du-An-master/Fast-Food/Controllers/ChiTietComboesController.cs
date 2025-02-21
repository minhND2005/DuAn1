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
    public class ChiTietComboesController : Controller
    {
        private readonly DoAnStoreContext _context;

        public ChiTietComboesController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: ChiTietComboes
        public async Task<IActionResult> Index()
        {
            var doAnStoreContext = _context.ChiTietCombos.Include(c => c.MaMonNavigation);
            return View(await doAnStoreContext.ToListAsync());
        }

        // GET: ChiTietComboes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietCombo = await _context.ChiTietCombos
                .Include(c => c.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaCombo == id);
            if (chiTietCombo == null)
            {
                return NotFound();
            }

            return View(chiTietCombo);
        }

        // GET: ChiTietComboes/Create
        public IActionResult Create()
        {
            // Truyền danh sách Combo (các món ăn loại Combo)
            ViewData["MaCombo"] = new SelectList(_context.MonAns
                .Where(m => m.LoaiSanPham == "7")
                .Select(m => new { m.MaMon, m.TenMon }), "MaMon", "TenMon");

            // Truyền danh sách Món ăn (các món ăn không phải Combo)
            ViewData["MaMonItems"] = new SelectList(_context.MonAns
                .Where(m => m.LoaiSanPham != "7")
                .Select(m => new { m.MaMon, m.TenMon }), "MaMon", "TenMon");

            return View();
        }

        // POST: ChiTietComboes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCombo,MaMon")] ChiTietCombo chiTietCombo)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu cả MaCombo và MaMon tồn tại trong bảng MonAn
                var isComboExists = _context.MonAns.Any(m => m.MaMon == chiTietCombo.MaCombo && m.LoaiSanPham == "7");
                var isMonExists = _context.MonAns.Any(m => m.MaMon == chiTietCombo.MaMon && m.LoaiSanPham != "7");

                if (isComboExists && isMonExists)
                {
                    // Thêm vào cơ sở dữ liệu
                        _context.Add(chiTietCombo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Combo hoặc Món ăn không tồn tại hoặc không hợp lệ.");
                }
            }

            // Nếu Model không hợp lệ, truyền lại dữ liệu vào dropdown
            ViewData["MaCombo"] = new SelectList(_context.MonAns
                .Where(m => m.LoaiSanPham == "7")
                .Select(m => new { m.MaMon, m.TenMon }), "MaMon", "TenMon", chiTietCombo.MaCombo);

            ViewData["MaMonItems"] = new SelectList(_context.MonAns
                .Where(m => m.LoaiSanPham != "7")
                .Select(m => new { m.MaMon, m.TenMon }), "MaMon", "TenMon", chiTietCombo.MaMon);

            return View(chiTietCombo);
        }

        // GET: ChiTietComboes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietCombo = await _context.ChiTietCombos.FindAsync(id);
            if (chiTietCombo == null)
            {
                return NotFound();
            }
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", chiTietCombo.MaMon);
            return View(chiTietCombo);
        }

        // POST: ChiTietComboes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaCombo,MaMon")] ChiTietCombo chiTietCombo)
        {
            if (id != chiTietCombo.MaCombo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTietCombo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietComboExists(chiTietCombo.MaCombo))
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
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", chiTietCombo.MaMon);
            return View(chiTietCombo);
        }

        // GET: ChiTietComboes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietCombo = await _context.ChiTietCombos
                .Include(c => c.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaCombo == id);
            if (chiTietCombo == null)
            {
                return NotFound();
            }

            return View(chiTietCombo);
        }

        // POST: ChiTietComboes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiTietCombo = await _context.ChiTietCombos.FindAsync(id);
            if (chiTietCombo != null)
            {
                _context.ChiTietCombos.Remove(chiTietCombo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietComboExists(int id)
        {
            return _context.ChiTietCombos.Any(e => e.MaCombo == id);
        }
    }
}
