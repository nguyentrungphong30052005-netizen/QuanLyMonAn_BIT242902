using Microsoft.EntityFrameworkCore;
using QuanLyMonAn_BIT242902.Models;

namespace QuanLyMonAn_BIT242902.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DishCategory_BIT242902> DishCategories { get; set; }
        public DbSet<Dish_BIT242902> Dishes { get; set; }
        public DbSet<DishImage_BIT242902> DishImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed dữ liệu 3 loại món
            modelBuilder.Entity<DishCategory_BIT242902>().HasData(
                new DishCategory_BIT242902 { Id = 1, Name = "Món chính", Description = "Các món chính trong bữa ăn" },
                new DishCategory_BIT242902 { Id = 2, Name = "Món khai vị", Description = "Khai vị nhẹ nhàng" },
                new DishCategory_BIT242902 { Id = 3, Name = "Tráng miệng", Description = "Đồ ngọt và trái cây" }
            );

            // Seed dữ liệu 5 món ăn
            modelBuilder.Entity<Dish_BIT242902>().HasData(
                new Dish_BIT242902 { Id = 1, Name = "Phở Bò Đặc Biệt", Price = 65000, PreparationTime = 12, IsAvailable = true, Description = "Phở truyền thống", DishCategoryId = 1 },
                new Dish_BIT242902 { Id = 2, Name = "Cơm Chiên Dương Châu", Price = 55000, PreparationTime = 18, IsAvailable = true, Description = "Cơm chiên thập cẩm", DishCategoryId = 1 },
                new Dish_BIT242902 { Id = 3, Name = "Gỏi Cuốn Tôm Thịt", Price = 40000, PreparationTime = 10, IsAvailable = true, Description = "Thanh mát", DishCategoryId = 2 },
                new Dish_BIT242902 { Id = 4, Name = "Súp Hải Sản", Price = 45000, PreparationTime = 15, IsAvailable = true, Description = "Ấm nóng", DishCategoryId = 2 },
                new Dish_BIT242902 { Id = 5, Name = "Chè Xoài Thái Lan", Price = 35000, PreparationTime = 8, IsAvailable = false, Description = "Ngọt mát", DishCategoryId = 3 }
            );

            // Seed dữ liệu ảnh
            modelBuilder.Entity<DishImage_BIT242902>().HasData(
                new DishImage_BIT242902 { Id = 1, ImageUrl = "/images/pho.jpg", IsThumbnail = true, DishId = 1 },
                new DishImage_BIT242902 { Id = 2, ImageUrl = "/images/comchien.jpg", IsThumbnail = true, DishId = 2 },
                new DishImage_BIT242902 { Id = 3, ImageUrl = "/images/goicuon.jpg", IsThumbnail = true, DishId = 3 },
                new DishImage_BIT242902 { Id = 4, ImageUrl = "/images/suphaisan.jpg", IsThumbnail = true, DishId = 4 },
                new DishImage_BIT242902 { Id = 5, ImageUrl = "/images/chexoai.jpg", IsThumbnail = true, DishId = 5 }
            );
        }
    }
}