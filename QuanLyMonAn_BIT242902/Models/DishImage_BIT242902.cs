using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyMonAn_BIT242902.Models
{
    [Table("BIT242902_DishImages")]
    public class DishImage_BIT242902
    {
        [Key]
        [Column("BIT242902_Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh không được để trống")]
        [Column("BIT242902_ImageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("BIT242902_IsThumbnail")]
        public bool IsThumbnail { get; set; } = false;

        [Column("BIT242902_DishId")]
        public int DishId { get; set; }

        [ForeignKey("DishId")]
        public Dish_BIT242902? Dish { get; set; }
    }
}