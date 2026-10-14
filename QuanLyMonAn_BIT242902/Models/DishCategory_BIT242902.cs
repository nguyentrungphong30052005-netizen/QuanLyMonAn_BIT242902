using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyMonAn_BIT242902.Models
{
    [Table("BIT242902_DishCategories")]
    public class DishCategory_BIT242902
    {
        [Key]
        [Column("BIT242902_Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên loại món không được để trống")]
        [Column("BIT242902_Name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("BIT242902_Description")]
        public string? Description { get; set; }

        public ICollection<Dish_BIT242902>? Dishes { get; set; }
    }
}