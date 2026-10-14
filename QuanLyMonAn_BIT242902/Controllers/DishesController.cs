using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyMonAn_BIT242902.Data;
using QuanLyMonAn_BIT242902.Models;

namespace QuanLyMonAn_BIT242902.Controllers
{
    public class DishesController : Controller
    {
        private readonly AppDbContext _context;

        public DishesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dishes
        public async Task<IActionResult> Index(string searchString, int? categoryId, bool? isAvailable, decimal? minPrice, decimal? maxPrice, string sortOrder)
        {
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentAvailable"] = isAvailable;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["PriceSortParm"] = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            ViewData["TimeSortParm"] = sortOrder == "time_asc" ? "time_desc" : "time_asc";

            var query = _context.Dishes
                .Include(d => d.DishCategory)
                .Include(d => d.DishImages)
                .AsQueryable();

            if (isAvailable.HasValue)
            {
                query = query.Where(d => d.IsAvailable == isAvailable.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(d => d.Name.Contains(searchString));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(d => d.DishCategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(d => d.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(d => d.Price <= maxPrice.Value);
            }

            query = sortOrder switch
            {
                "price_desc" => query.OrderByDescending(d => d.Price),
                "time_asc" => query.OrderBy(d => d.PreparationTime),
                "time_desc" => query.OrderByDescending(d => d.PreparationTime),
                _ => query.OrderBy(d => d.Price),
            };

            ViewBag.Categories = new SelectList(await _context.DishCategories.ToListAsync(), "Id", "Name", categoryId);
            return View(await query.ToListAsync());
        }

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound("Mã món ăn không hợp lệ.");

            var dish = await _context.Dishes
                .Include(d => d.DishCategory)
                .Include(d => d.DishImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dish == null) return NotFound($"Không tìm thấy món ăn với ID = {id}.");

            return View(dish);
        }

        // GET: Dishes/Create
        public IActionResult Create()
        {
            ViewData["DishCategoryId"] = new SelectList(_context.DishCategories, "Id", "Name");
            return View();
        }

        // POST: Dishes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dish_BIT242902 dish, string imageUrl)
        {
            bool isDuplicate = await _context.Dishes.AnyAsync(d => d.Name == dish.Name && d.DishCategoryId == dish.DishCategoryId);
            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "Tên món ăn đã tồn tại trong loại món này.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(dish);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var urls = imageUrl.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    bool isFirst = true;
                    foreach (var url in urls)
                    {
                        var image = new DishImage_BIT242902
                        {
                            ImageUrl = url.Trim(),
                            IsThumbnail = isFirst,
                            DishId = dish.Id
                        };
                        _context.Add(image);
                        isFirst = false;
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["DishCategoryId"] = new SelectList(_context.DishCategories, "Id", "Name", dish.DishCategoryId);
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound("Mã món ăn không hợp lệ.");

            var dish = await _context.Dishes
                .Include(d => d.DishImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dish == null) return NotFound($"Không tìm thấy món ăn với ID = {id}.");

            ViewData["DishCategoryId"] = new SelectList(_context.DishCategories, "Id", "Name", dish.DishCategoryId);
            return View(dish);
        }

        // POST: Dishes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dish_BIT242902 dish, string imageUrl)
        {
            if (id != dish.Id) return NotFound();

            var dishToUpdate = await _context.Dishes
                .Include(d => d.DishImages)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dishToUpdate == null) return NotFound();

            // Cập nhật thông tin cơ bản
            dishToUpdate.Name = dish.Name;
            dishToUpdate.DishCategoryId = dish.DishCategoryId;
            dishToUpdate.Price = dish.Price;
            dishToUpdate.PreparationTime = dish.PreparationTime;
            dishToUpdate.IsAvailable = dish.IsAvailable;
            dishToUpdate.Description = dish.Description;

            try
            {
                // Nếu có nhập thêm ảnh mới thì thêm vào bảng DishImages
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    bool hasThumbnail = dishToUpdate.DishImages != null && dishToUpdate.DishImages.Any(img => img.IsThumbnail);

                    var newImage = new DishImage_BIT242902
                    {
                        ImageUrl = imageUrl.Trim(),
                        IsThumbnail = !hasThumbnail, // Nếu chưa có ảnh đại diện nào thì ảnh này làm đại diện
                        DishId = dishToUpdate.Id
                    };
                    _context.Add(newImage);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Dishes.Any(e => e.Id == dish.Id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // HÀM ĐẶT ẢNH LÀM THUMBNAIL (ĐỔI ẢNH ĐẠI DIỆN)
        [HttpPost]
        public async Task<IActionResult> SetThumbnail(int imageId, int dishId)
        {
            var images = await _context.DishImages.Where(img => img.DishId == dishId).ToListAsync();
            foreach (var img in images)
            {
                img.IsThumbnail = (img.Id == imageId);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = dishId });
        }

        // HÀM XÓA MỘT ẢNH TRONG KHI ĐANG SỬA
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId, int dishId)
        {
            var image = await _context.DishImages.FindAsync(imageId);
            if (image != null)
            {
                bool wasThumbnail = image.IsThumbnail;
                _context.DishImages.Remove(image);
                await _context.SaveChangesAsync();

                if (wasThumbnail)
                {
                    var nextImage = await _context.DishImages.FirstOrDefaultAsync(img => img.DishId == dishId);
                    if (nextImage != null)
                    {
                        nextImage.IsThumbnail = true;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("Edit", new { id = dishId });
        }

        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound("Mã món ăn không hợp lệ.");

            var dish = await _context.Dishes
                .Include(d => d.DishCategory)
                .Include(d => d.DishImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dish == null) return NotFound($"Không tìm thấy món ăn với ID = {id}.");

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.DishImages)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish != null)
            {
                if (dish.DishImages != null && dish.DishImages.Any())
                {
                    _context.DishImages.RemoveRange(dish.DishImages);
                }

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}