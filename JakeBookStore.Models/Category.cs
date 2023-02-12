using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JakeDrinkStore.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
