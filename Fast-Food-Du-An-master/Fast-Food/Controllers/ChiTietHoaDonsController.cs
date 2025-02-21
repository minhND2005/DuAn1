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
    public class ChiTietHoaDonsController : Controller
    {
        private readonly DoAnStoreContext _context;

        public ChiTietHoaDonsController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: ChiTietHoaDons
        public async Task<IActionResult> Index()
        {
            var doAnStoreContext = _context.ChiTietHoaDons.Include(c => c.MaHoaDonNavigation).Include(c => c.MaMonNavigation);
            return View(await doAnStoreContext.ToListAsync());
        }

        // GET: ChiTietHoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDons
                .Include(c => c.MaHoaDonNavigation)
                .Include(c => c.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaChiTietHoaDon == id);
            if (chiTietHoaDon == null)
            {
                return NotFound();
            }

            return View(chiTietHoaDon);
        }

        // GET: ChiTietHoaDons/Create
        public IActionResult Create()
        {
            ViewData["MaHoaDon"] = new SelectList(_context.HoaDons, "MaHoaDon", "MaHoaDon");
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon");
            return View();
        }

        // POST: ChiTietHoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaChiTietHoaDon,MaMon,MaHoaDon,TenMonAn,SoLuong,Gia,TongTien,GhiChu")] ChiTietHoaDon chiTietHoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chiTietHoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHoaDon"] = new SelectList(_context.HoaDons, "MaHoaDon", "MaHoaDon", chiTietHoaDon.MaHoaDon);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", chiTietHoaDon.MaMon);
            return View(chiTietHoaDon);
        }

        // GET: ChiTietHoaDons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDons.FindAsync(id);
            if (chiTietHoaDon == null)
            {
                return NotFound();
            }
            ViewData["MaHoaDon"] = new SelectList(_context.HoaDons, "MaHoaDon", "MaHoaDon", chiTietHoaDon.MaHoaDon);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", chiTietHoaDon.MaMon);
            return View(chiTietHoaDon);
        }

        // POST: ChiTietHoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaChiTietHoaDon,MaMon,MaHoaDon,TenMonAn,SoLuong,Gia,TongTien,GhiChu")] ChiTietHoaDon chiTietHoaDon)
        {
            if (id != chiTietHoaDon.MaChiTietHoaDon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTietHoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietHoaDonExists(chiTietHoaDon.MaChiTietHoaDon))
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
            ViewData["MaHoaDon"] = new SelectList(_context.HoaDons, "MaHoaDon", "MaHoaDon", chiTietHoaDon.MaHoaDon);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", chiTietHoaDon.MaMon);
            return View(chiTietHoaDon);
        }

        // GET: ChiTietHoaDons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDons
                .Include(c => c.MaHoaDonNavigation)
                .Include(c => c.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaChiTietHoaDon == id);
            if (chiTietHoaDon == null)
            {
                return NotFound();
            }

            return View(chiTietHoaDon);
        }

        // POST: ChiTietHoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiTietHoaDon = await _context.ChiTietHoaDons.FindAsync(id);
            if (chiTietHoaDon != null)
            {
                _context.ChiTietHoaDons.Remove(chiTietHoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietHoaDonExists(int id)
        {
            return _context.ChiTietHoaDons.Any(e => e.MaChiTietHoaDon == id);
        }
    }
}
