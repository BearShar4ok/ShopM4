using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopM4_Models
{
    public class MyModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be more than 0")]
        public int Number { get; set; }
    }
}
