using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyMonAn_BIT242902.Models
{
    [Table("BIT242902_Dishes")]
    public class Dish_BIT242902
    {
        [Key]
        [Column("BIT242902_Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên món ăn không được để trống")]
        [Column("BIT242902_Name")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column("BIT242902_Price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thời gian chế biến phải lớn hơn 0")]
        [Column("BIT242902_PreparationTime")]
        public int PreparationTime { get; set; }

        [Column("BIT242902_IsAvailable")]
        public bool IsAvailable { get; set; } = true;

        [Column("BIT242902_Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại món ăn")]
        [Column("BIT242902_DishCategoryId")]
        public int DishCategoryId { get; set; }

        [ForeignKey("DishCategoryId")]
        public DishCategory_BIT242902? DishCategory { get; set; }

        public ICollection<DishImage_BIT242902>? DishImages { get; set; }
    }
}