using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyMonAn_BIT242902.Data;
using QuanLyMonAn_BIT242902.Models;

namespace QuanLyMonAn_BIT242902.Controllers
{
    public class DishCategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public DishCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DishCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.DishCategories.Include(c => c.Dishes).ToListAsync());
        }

        // GET: DishCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound("Mã loại món không hợp lệ.");

            var category = await _context.DishCategories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) return NotFound($"Không tìm thấy loại món với ID = {id}.");

            return View(category);
        }

        // GET: DishCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DishCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DishCategory_BIT242902 category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: DishCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound("Mã loại món không hợp lệ.");

            var category = await _context.DishCategories.FindAsync(id);
            if (category == null) return NotFound($"Không tìm thấy loại món với ID = {id}.");

            return View(category);
        }

        // POST: DishCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DishCategory_BIT242902 category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DishCategories.Any(e => e.Id == category.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: DishCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound("Mã loại món không hợp lệ.");

            var category = await _context.DishCategories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) return NotFound($"Không tìm thấy loại món với ID = {id}.");

            return View(category);
        }

        // POST: DishCategories/Delete/5 (CHẶN XÓA NẾU ĐANG CÓ MÓN ĂN)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.DishCategories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                // Tiêu chí 5: Kiểm tra nếu loại món đang chứa món ăn thì không cho xóa, báo lỗi ra giao diện
                if (category.Dishes != null && category.Dishes.Any())
                {
                    ModelState.AddModelError(string.Empty, "Không thể xóa loại món này vì vẫn còn món ăn đang thuộc danh mục này!");
                    return View("Delete", category);
                }

                _context.DishCategories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}