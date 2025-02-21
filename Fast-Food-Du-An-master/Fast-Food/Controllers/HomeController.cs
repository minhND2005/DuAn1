using System.Diagnostics;
using Fast_Food.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fast_Food.Controllers
{
    public class HomeController : Controller
    {
        readonly DoAnStoreContext _context;

        // Constructor để khởi tạo _context thông qua dependency injection
        public HomeController(DoAnStoreContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var monAnList = _context.MonAns
                   .AsEnumerable() // Chuyển sang bộ nhớ
                   .GroupBy(m => m.LoaiSanPham) // Nhóm theo loại sản phẩm
                   .Select(g => g.Take(6)) // Chỉ lấy 10 món mới nhất trong mỗi nhóm
                   .SelectMany(g => g) // Flatten danh sách
                   .ToList();
            return View(monAnList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
